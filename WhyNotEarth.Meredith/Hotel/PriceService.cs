using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Hotel
{
    public class PriceService
    {
        protected MeredithDbContext Context { get; }

        public PriceService(MeredithDbContext meredithDbContext)
        {
            Context = meredithDbContext;
        }

        public async Task<HotelPrice> CreatePriceAsync(decimal amount, DateTime date, int roomTypeId)
        {
            var roomType = await Context.RoomTypes.SingleOrDefaultAsync(a => a.Id == roomTypeId);

            if (roomType == null)
            {
                throw new InvalidActionException("Roomtype does not exist");
            }

            if (amount < 0)
            {
                throw new InvalidActionException("Amount cannot be below zero");
            }

            var price = new HotelPrice
            {
                Amount = amount,
                Date = date,
                RoomTypeId = roomTypeId
            };

            Context.Prices.Add(price);
            await Context.SaveChangesAsync();
            return price;
        }
    }
}