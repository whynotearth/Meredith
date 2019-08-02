namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.App.Models.Api.v0.Reservation;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Data.Entity.Models;
    using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
    using WhyNotEarth.Meredith.Stripe;

    [ApiVersion("0")]
    [Route("/api/v0/reservations")]
    [EnableCors]
    public class ReservationController : Controller
    {
        private MeredithDbContext MeredithDbContext { get; }

        private StripeService StripeService { get; }

        private UserManager<User> UserManager { get; }

        public ReservationController(
            MeredithDbContext meredithDbContext,
            UserManager<User> userManager,
            StripeService stripeService)
        {
            MeredithDbContext = meredithDbContext;
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
            return Ok(reservation);
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
                    Amount = Convert.ToDecimal(model.Amount) / 100m,
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

            var startDate = model.Start.Date;
            var endDate = model.End.Date;
            var roomType = await MeredithDbContext.RoomTypes
                .Where(rt => rt.Id == roomTypeId)
                .Select(rt => new
                {
                    Price = rt.Prices
                        .Where(p => p.Date >= startDate && p.Date < endDate)
                        .Sum(p => p.Amount),
                    PaidDays = rt.Prices
                        .Where(p => p.Date >= startDate && p.Date < endDate)
                        .Count(),
                    AvailableRooms = rt.Rooms
                        .Where(r => !r.Reservations
                            .Any(re => re.Start >= startDate && re.End <= endDate))
                        .ToList()
                })
                .FirstOrDefaultAsync();
            if (roomType.AvailableRooms.Count == 0)
            {
                return BadRequest("There are no rooms available of this type");
            }

            var totalDays = endDate.Subtract(startDate).TotalDays;
            if (totalDays <= 0)
            {
                return BadRequest("Invalid number of days to reserve");
            }

            if (roomType.PaidDays != totalDays)
            {
                return BadRequest("Not all days have prices set");
            }

            var user = await UserManager.GetUserAsync(User);
            var reservation = new Reservation
            {
                Amount = roomType.Price,
                Created = DateTime.UtcNow,
                Start = startDate,
                End = endDate,
                Email = model.Email,
                Message = model.Message,
                Name = model.Name,
                NumberOfGuests = model.NumberOfGuests,
                Phone = model.Phone,
                RoomId = roomType.AvailableRooms.First().Id,
                User = user
            };
            MeredithDbContext.Reservations.Add(reservation);
            await MeredithDbContext.SaveChangesAsync();
            return Ok(new
            {
                reservationId = reservation.Id
            });
        }
    }
}