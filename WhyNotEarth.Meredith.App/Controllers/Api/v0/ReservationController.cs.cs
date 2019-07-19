namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
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

    [ApiVersion("0")]
    [Route("/api/v0/reservations")]
    [EnableCors]
    public class ReservationController : Controller
    {
        private MeredithDbContext MeredithDbContext { get; }

        private UserManager<User> UserManager { get; }

        public ReservationController(
            MeredithDbContext meredithDbContext,
            UserManager<User> userManager)
        {
            MeredithDbContext = meredithDbContext;
            UserManager = userManager;
        }

        [HttpGet]
        [Route("{reservationId}")]
        public async Task<IActionResult> Get(int reservationId)
        {
            return Ok();
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
                Start = startDate,
                End = endDate,
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