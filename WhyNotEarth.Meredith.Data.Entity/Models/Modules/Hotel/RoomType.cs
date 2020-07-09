#nullable enable

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    public class RoomType
    {
        public int Id { get; set; }

        public ICollection<Bed> Beds { get; set; } = new List<Bed>();

        public int Capacity { get; set; }

        public int HotelId { get; set; }

        public Hotel Hotel { get; set; } = null!;

        public string? Name { get; set; }

        public ICollection<HotelPrice> Prices { get; set; } = new List<HotelPrice>();

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }

    public class RoomTypeEntityConfig : IEntityTypeConfiguration<RoomType>
    {
        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            builder.Property(e => e.Name);
            builder.ToTable("RoomTypes", "ModuleHotel");
        }
    }
}