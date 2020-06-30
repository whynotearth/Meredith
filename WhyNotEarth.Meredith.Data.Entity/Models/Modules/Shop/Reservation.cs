#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public abstract class Reservation
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;
        
        public DateTime Created { get; set; }

        public string? Email { get; set; }

        public string? Message { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? GeoLocation { get; set; }

        public string? AddressFriendlyName { get; set; }

        public string? StreetAddress { get; set; }
        
        public string? City { get; set; }

        public string? Floor { get; set; }
        
        public string? Apartment { get; set; }

        public string? WhereToPark { get; set; }

        public DateTime DeliveryDateTime { get; set; }

        public ICollection<Payment>? Payments { get; set; }
    }

    public class ReservationEntityConfig : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations", "ModuleShop");
            builder.Property(e => e.Amount).HasColumnType("numeric(10, 2)");
        }
    }
}