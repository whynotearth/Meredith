namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Amenity : IEntityTypeConfiguration<Amenity>
    {
        public Hotel Hotel { get; set; }

        public int HotelId { get; set; }

        public int Id { get; set; }

        public string Text { get; set; }

        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.ToTable("Amenities", "ModuleHotel");
        }
    }
}