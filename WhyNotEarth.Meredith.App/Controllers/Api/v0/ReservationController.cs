namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.App.Models.Api.v0.Reservation;
    using WhyNotEarth.Meredith.App.Results.Api.v0.Reservation;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Hotel;
    using WhyNotEarth.Meredith.Identity;

    [ApiVersion("0")]
    [Route("/api/v0/reservations")]
    [EnableCors]
    public class ReservationController : Controller
    {
        private MeredithDbContext MeredithDbContext { get; }

        private ReservationService ReservationService { get; }

        private UserManager UserManager { get; }

        public ReservationController(
            MeredithDbContext meredithDbContext,
            UserManager userManager,
            ReservationService reservationService)
        {
            MeredithDbContext = meredithDbContext;
            ReservationService = reservationService;
            UserManager = userManager;
        }

        [Authorize]
        [HttpGet]
        [Route("{reservationId}")]
        public async Task<IActionResult> Get(int reservationId)
        {
            var user = await UserManager.GetUserAsync(User);
            var reservation = await MeredithDbContext.Reservations
                .Include(r => r.Payments)
                .Where(r => r.Id == reservationId && r.UserId == user.Id)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                reservation.Created,
                reservation.Email,
                reservation.End,
                reservation.Id,
                reservation.Message,
                reservation.Name,
                reservation.NumberOfGuests,
                Payments = reservation.Payments.Select(p => new
                {
                    p.Amount,
                    p.Created,
                    p.Id,
                    p.Status,
                    p.UserId,
                }).ToList(),
                reservation.Phone,
                reservation.RoomId,
                reservation.Start,
                reservation.UserId
            });
        }

        [Authorize]
        [HttpPost]
        [Route("{reservationId}/pay")]
        [ProducesResponseType(typeof(PayReservationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PayReservation(int reservationId, [FromBody] PayModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var reservation = await MeredithDbContext.Reservations.FirstOrDefaultAsync(item => item.Id == reservationId);

            if (reservation is null)
            {
                return NotFound();
            }

            var clientSecret = await ReservationService.PayReservation(reservationId, reservation.Amount, model.Metadata);

            return Ok(new PayReservationResult(clientSecret));
        }

        [Authorize]
        [HttpPost]
        [Route("roomtype/{roomTypeId}/reserve")]
        public async Task<IActionResult> ReserveByRoomType(int roomTypeId, [FromBody] ReservationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var reservation = await ReservationService.CreateReservation(
                roomTypeId, model.Start, model.End, model.Name, model.Email, model.Message, model.PhoneCountry, model.Phone,
                model.NumberOfGuests);

            return Ok(new
            {
                reservationId = reservation.Id
            });
        }

        [HttpPost]
        [Route("paymentintent/confirm")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmPaymentIntent()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            await ReservationService.ConfirmPayment(json, Request.Headers["Stripe-Signature"]);

            return Ok();
        }
    }
}