using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class Payment : IEntityTypeConfiguration<Payment>
    {
        public int Id { get; set; }

        public int ReservationId { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public decimal Amount { get; set; }

        public string PaymentIntentId { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime Created { get; set; }

        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments", "ModuleShop");
        }
    }

    public enum PaymentStatus
    {
        IntentGenerated,
        Fulfilled
    }
}