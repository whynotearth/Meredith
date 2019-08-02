namespace WhyNotEarth.Meredith.Hotel
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
    using WhyNotEarth.Meredith.Exceptions;
    using WhyNotEarth.Meredith.Identity;

    public class ReservationService
    {
        protected MeredithDbContext MeredithDbContext { get; }

        protected ClaimsPrincipal User { get; }


        protected UserManager UserManager { get; }

        public ReservationService(
            MeredithDbContext meredithDbContext,
            ClaimsPrincipal user,
            UserManager userManager)
        {
            MeredithDbContext = meredithDbContext;
            User = user;
            UserManager = userManager;
        }

        public async Task<Reservation> CreateReservation(
            int roomTypeId,
            DateTime startDate,
            DateTime endDate,
            string fullName,
            string email,
            string message,
            string phone,
            int numberOfGuests)
        {
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
            if (roomType == null)
            {
                throw new RecordNotFoundException();
            }

            if (roomType.AvailableRooms.Count == 0)
            {
                throw new InvalidActionException("There are no rooms available of this type");
            }

            var totalDays = endDate.Subtract(startDate).TotalDays;
            if (totalDays <= 0)
            {
                throw new InvalidActionException("Invalid number of days to reserve");
            }

            if (roomType.PaidDays != totalDays)
            {
                throw new InvalidActionException("Not all days have prices set");
            }

            var user = await UserManager.GetUserAsync(User);
            var reservation = new Reservation
            {
                Amount = roomType.Price,
                Created = DateTime.UtcNow,
                Start = startDate,
                End = endDate,
                Email = email,
                Message = message,
                Name = fullName,
                NumberOfGuests = numberOfGuests,
                Phone = phone,
                RoomId = roomType.AvailableRooms.First().Id,
                User = user
            };
            MeredithDbContext.Reservations.Add(reservation);
            await MeredithDbContext.SaveChangesAsync();
            return reservation;
        }
    }
}