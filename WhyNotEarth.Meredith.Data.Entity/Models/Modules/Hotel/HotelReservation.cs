using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    public class HotelReservation : Reservation
    {
        public int RoomId { get; set; }

        public Room Room { get; set; }

        public DateTime Start { get; set; }
        
        public DateTime End { get; set; }

        public int NumberOfGuests { get; set; }

        public void Configure(EntityTypeBuilder<HotelReservation> builder)
        {
            builder.Property(e => e.Start).HasColumnType("date");
            builder.Property(e => e.End).HasColumnType("date");
        }
    }
}