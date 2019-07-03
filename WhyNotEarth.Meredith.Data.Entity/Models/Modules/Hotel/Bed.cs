namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Bed : IEntityTypeConfiguration<Bed>
    {
        public enum BedTypes
        {
            King,
            Queen,
            Twin,
            Single
        };

        public BedTypes BedType { get; set; }

        public int Count { get; set; }

        public Hotel Hotel { get; set; }

        public int HotelId { get; set; }

        public int Id { get; set; }

        public void Configure(EntityTypeBuilder<Bed> builder)
        {
            builder.ToTable("Beds", "ModuleHotel");
        }
    }
}