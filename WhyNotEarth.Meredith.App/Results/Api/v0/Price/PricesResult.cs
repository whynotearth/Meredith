using System;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Price
{
    public class PricesResult
    {
        public int Id { get; }

        public int RoomTypeId { get; }

        public DateTime Date { get; }

        public decimal Amount { get; }

        public PricesResult(int id, int roomTypeId, DateTime date, decimal amount)
        {
            Id = id;
            RoomTypeId = roomTypeId;
            Date = date;
            Amount = amount;
        }
    }
}