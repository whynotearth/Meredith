namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class RoomType : IEntityTypeConfiguration<RoomType>
    {
        public ICollection<Amenity> Amenities { get; set; }

        public ICollection<Bed> Beds { get; set; }

        public int Id { get; set; }

        public Hotel Hotel { get; set; }

        public int HotelId { get; set; }

        public string Name { get; set; }

        public ICollection<Price> Prices { get; set; }

        public ICollection<Room> Rooms { get; set; }

        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            builder.Property(e => e.Name).HasMaxLength(64);
            builder.ToTable("RoomTypes", "ModuleHotel");
        }
    }
}