namespace WhyNotEarth.Meredith.Hotel
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
    using WhyNotEarth.Meredith.Exceptions;

    public class PriceService
    {
        protected MeredithDbContext Context { get; }

        public PriceService(
            MeredithDbContext meredithDbContext)
        {
            Context = meredithDbContext;
        }

        public async Task<Price> CreatePriceAsync(decimal amount, DateTime date, int roomTypeId)
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
