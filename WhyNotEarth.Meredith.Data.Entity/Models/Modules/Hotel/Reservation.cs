namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Reservation : IEntityTypeConfiguration<Reservation>
    {
        public decimal Amount { get; set; }

        public DateTime Created { get; set; }

        public DateTime End { get; set; }

        public int Id { get; set; }

        public ICollection<Payment> Payments { get; set; }

        public Room Room { get; set; }

        public int RoomId { get; set; }

        public DateTime Start { get; set; }

        public User User { get; set; }

        public int UserId { get; set; }

        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.Property(e => e.Amount).HasColumnType("numeric(10, 2)");
            builder.Property(e => e.Start).HasColumnType("date");
            builder.Property(e => e.End).HasColumnType("date");
            builder.ToTable("Reservations", "ModuleHotel");
        }
    }
}