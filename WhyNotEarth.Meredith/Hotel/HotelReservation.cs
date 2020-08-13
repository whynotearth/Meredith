using System;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.Hotel
{
    public class HotelReservation : Reservation
    {
        public int RoomId { get; set; }

        public Room Room { get; set; } = null!;

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public int NumberOfGuests { get; set; }
    }
}