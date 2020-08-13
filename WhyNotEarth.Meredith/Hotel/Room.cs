using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Hotel
{
    public class Room
    {
        public int Id { get; set; }

        public int RoomTypeId { get; set; }

        public RoomType RoomType { get; set; } = null!;

        public string? Number { get; set; }

        public ICollection<HotelReservation> Reservations { get; set; } = null!;
    }
}