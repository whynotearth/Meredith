using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Hotel
{
    public class PriceService
    {
        protected MeredithDbContext Context { get; }

        public PriceService(
            MeredithDbContext meredithDbContext)
        {
            Context = meredithDbContext;
        }

        public async Task<Price> CreatePriceAsync(int amount, DateTime date, int roomTypeId)
        {
            var roomType = await Context.RoomTypes.SingleOrDefaultAsync(a => a.Id == roomTypeId);

            if (roomType == null)
                throw new ArgumentException(string.Format("Roomtype {0} does not exist", roomTypeId));

            if (amount < 0)
                throw new InvalidActionException("Amount cannot be below zero");

            var price = new Price
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
