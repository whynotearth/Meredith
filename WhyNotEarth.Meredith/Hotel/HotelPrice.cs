using System;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.Hotel
{
    public class HotelPrice : Price
    {
        public DateTime Date { get; set; }

        public int RoomTypeId { get; set; }

        public RoomType RoomType { get; set; } = null!;
    }
}