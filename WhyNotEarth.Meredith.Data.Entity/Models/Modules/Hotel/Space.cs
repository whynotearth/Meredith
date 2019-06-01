namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Space : IEntityTypeConfiguration<Space>
    {
        public Hotel Hotel { get; set; }

        public Guid HotelId { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public void Configure(EntityTypeBuilder<Space> builder)
        {
            builder.ToTable("Spaces", "ModuleHotel");
        }
    }
}