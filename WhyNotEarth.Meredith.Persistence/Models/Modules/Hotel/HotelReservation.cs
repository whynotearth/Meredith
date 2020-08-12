using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using WhyNotEarth.Meredith.Persistence.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class HotelReservation : Reservation
    {
        public int RoomId { get; set; }

        public Room Room { get; set; } = null!;

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public int NumberOfGuests { get; set; }
    }

    public class HotelReservationEntityConfig : ReservationEntityConfig
    {
        public void Configure(EntityTypeBuilder<HotelReservation> builder)
        {
            builder.Property(e => e.Start).HasColumnType("date");
            builder.Property(e => e.End).HasColumnType("date");
        }
    }
}