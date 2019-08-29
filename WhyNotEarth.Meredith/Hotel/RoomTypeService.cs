namespace WhyNotEarth.Meredith.Hotel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Exceptions;
    using WhyNotEarth.Meredith.Hotel.Data;
    using WhyNotEarth.Meredith.Identity;

    public class RoomTypeService
    {
        protected MeredithDbContext MeredithDbContext { get; }

        protected ClaimsPrincipal User { get; }

        protected UserManager UserManager { get; }

        public RoomTypeService(
            MeredithDbContext meredithDbContext,
            ClaimsPrincipal user,
            UserManager userManager)
        {
            MeredithDbContext = meredithDbContext;
            User = user;
            UserManager = userManager;
        }

        public async Task<List<Availability>> GetAvailabilitiesAsync(int roomTypeId, DateTime startDate, DateTime endDate)
        {
            CheckDates(startDate, endDate);
            var availabilities = await MeredithDbContext.Prices
                .Where(p => p.RoomTypeId == roomTypeId)
                .Select(p => new Availability
                {
                    Availabile = p.RoomType.Rooms
                        .All(room => room.Reservations
                            .Any(reservation => reservation.Start <= p.Date && reservation.End >= p.Date)),
                    Date = p.Date
                })
                .ToListAsync();
            return availabilities;
        }

        public async Task<List<PriceModel>> GetPrices(int roomTypeId, DateTime startDate, DateTime endDate)
        {
            CheckDates(startDate, endDate);
            return await MeredithDbContext.Prices
                .Where(p => p.RoomTypeId == roomTypeId
                    && p.Date >= startDate
                    && p.Date <= endDate)
                .Select(p => new PriceModel
                {
                    Id = p.Id,
                    RoomTypeId = p.RoomTypeId,
                    Date = p.Date,
                    Amount = p.Amount
                })
                .ToListAsync();
        }

        private void CheckDates(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new InvalidActionException("Start Date cannot be before End Date");
            }
        }
    }
}