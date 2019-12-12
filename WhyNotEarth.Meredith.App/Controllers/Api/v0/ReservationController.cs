namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.App.Models.Api.v0.Reservation;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Data.Entity.Models;
    using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
    using WhyNotEarth.Meredith.Hotel;
    using WhyNotEarth.Meredith.Identity;
    using WhyNotEarth.Meredith.Stripe;

    [ApiVersion("0")]
    [Route("/api/v0/reservations")]
    [EnableCors]
    public class ReservationController : Controller
    {
        private MeredithDbContext MeredithDbContext { get; }

        private ReservationService ReservationService { get; }

        private StripeService StripeService { get; }

        private UserManager UserManager { get; }

        public ReservationController(
            MeredithDbContext meredithDbContext,
            UserManager userManager,
            StripeService stripeService,
            ReservationService reservationService)
        {
            MeredithDbContext = meredithDbContext;
            ReservationService = reservationService;
            StripeService = stripeService;
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
        public async Task<IActionResult> PayReservation(int reservationId, [FromBody] PayModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var (reservation, company, user) = await GetReservationAsync(reservationId);
                if (reservation == null)
                {
                    return NotFound();
                }

                var paymentIntent = await StripeService.CreatePaymentIntent(company.Id, model.Amount, user.Email, new Dictionary<string, string>());
                var payment = new Payment
                {
                    Amount = model.Amount,
                    Created = DateTime.UtcNow,
                    ReservationId = reservation.Id,
                    Status = Payment.Statuses.IntentGenerated,
                    PaymentIntentId = paymentIntent.Id,
                    UserId = user.Id,
                };
                MeredithDbContext.Payments.Add(payment);
                await MeredithDbContext.SaveChangesAsync();
                return Ok(new
                {
                    paymentIntent.ClientSecret
                });
            }
            catch (Exception exception)
            {
                return StatusCode(500, new
                {
                    error = exception.Message
                });
            }
        }

        private async Task<(Reservation, Company, User)> GetReservationAsync(int reservationId)
        {
            var user = await UserManager.GetUserAsync(User);
            var results = await MeredithDbContext.Reservations
                .Where(r => r.Id == reservationId && r.UserId == user.Id)
                .Select(r => new
                {
                    Reservation = r,
                    r.Room.RoomType.Hotel.Company
                })
                .FirstOrDefaultAsync();
            if (results.Company == null)
            {
                throw new Exception("This hotel does not have a bound company");
            }

            return (results.Reservation, results.Company, user);
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
                roomTypeId, model.Start, model.End, model.Name, model.Email, model.Message, model.Phone, model.NumberOfGuests);
            return Ok(new
            {
                reservationId = reservation.Id
            });
        }
    }
}