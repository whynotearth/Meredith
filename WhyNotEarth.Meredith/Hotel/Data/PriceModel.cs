namespace WhyNotEarth.Meredith.Hotel.Data
{
    using System;

    public class PriceModel
    {
        public int Id { get; set; }

        public int RoomTypeId { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }
    }
}