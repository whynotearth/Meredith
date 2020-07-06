using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    public class Room
    {
        public int Id { get; set; }

        public RoomType RoomType { get; set; }

        public int RoomTypeId { get; set; }

        public string Number { get; set; }

        public ICollection<HotelReservation> Reservations { get; set; }
    }

    public class RoomEntityConfig : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.Property(e => e.Number).HasMaxLength(16);
            builder.ToTable("Rooms", "ModuleHotel");
        }
    }
}