namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Price : IEntityTypeConfiguration<Price>
    {
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public int Id { get; set; }

        public RoomType RoomType { get; set; }

        public int RoomTypeId { get; set; }

        public void Configure(EntityTypeBuilder<Price> builder)
        {
            builder.Property(e => e.Amount).HasColumnType("numeric(10, 2)");
            builder.Property(e => e.Date).HasColumnType("date");
            builder.ToTable("Prices", "ModuleHotel");
        }
    }
}