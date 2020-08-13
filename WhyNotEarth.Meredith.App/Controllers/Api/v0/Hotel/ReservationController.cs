using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.App.Models.Api.v0.Reservation;
using WhyNotEarth.Meredith.App.Results.Api.v0.Hotel.Reservation;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Hotel
{
    [ApiVersion("0")]
    [Route("api/v0/hotel/reservations")]
    [ProducesErrorResponseType(typeof(void))]
    public class ReservationController : ControllerBase
    {
        private readonly MeredithDbContext _meredithDbContext;
        private readonly ReservationService _reservationService;
        private readonly IUserService _userService;

        public ReservationController(MeredithDbContext meredithDbContext, IUserService userService,
            ReservationService reservationService)
        {
            _meredithDbContext = meredithDbContext;
            _userService = userService;
            _reservationService = reservationService;
        }

        [Authorize]
        [HttpGet("{reservationId}")]
        public async Task<IActionResult> Get(int reservationId)
        {
            var user = await _userService.GetUserAsync(User);
            var reservation = await _meredithDbContext.Reservations.OfType<HotelReservation>()
                .Include(r => r.Payments)
                .Where(r => r.Id == reservationId && r.UserId == user.Id)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                Created = reservation.CreatedAt,
                reservation.Email,
                reservation.End,
                reservation.Id,
                reservation.Message,
                reservation.Name,
                reservation.NumberOfGuests,
                Payments = reservation.Payments.Select(p => new
                {
                    p.Amount,
                    Created = p.CreatedAt,
                    p.Id,
                    p.Status,
                    p.UserId
                }).ToList(),
                reservation.Phone,
                reservation.RoomId,
                reservation.Start,
                reservation.UserId
            });
        }

        [Authorize]
        [Returns200]
        [Returns404]
        [HttpPost("{reservationId}/pay")]
        public async Task<ActionResult<PayReservationResult>> PayReservation(int reservationId, PayModel model)
        {
            var reservation =
                await _meredithDbContext.Reservations.FirstOrDefaultAsync(item => item.Id == reservationId);

            if (reservation is null)
            {
                return NotFound();
            }

            var clientSecret =
                await _reservationService.PayReservation(reservationId, reservation.Amount, model.Metadata);

            return Ok(new PayReservationResult(clientSecret));
        }

        [Authorize]
        [HttpPost("roomtype/{roomTypeId}/reserve")]
        public async Task<IActionResult> ReserveByRoomType(int roomTypeId, ReservationModel model)
        {
            var reservation = await _reservationService.CreateReservation(
                roomTypeId, model.Start, model.End, model.Name, model.Email, model.Message, model.PhoneCountry,
                model.Phone, model.NumberOfGuests);

            return Ok(new
            {
                reservationId = reservation.Id
            });
        }

        [HttpPost("paymentintent/confirm")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ConfirmPaymentIntent()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            await _reservationService.ConfirmPayment(json, Request.Headers["Stripe-Signature"]);

            return Ok();
        }
    }
}