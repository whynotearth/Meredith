using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Shop
{
    public class Payment
    {
        public int Id { get; set; }

        public int ReservationId { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public decimal Amount { get; set; }

        public string PaymentIntentId { get; set; } = null!;

        public PaymentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public enum PaymentStatus
    {
        IntentGenerated,
        Fulfilled
    }

    public class PaymentEntityConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments", "ModuleShop");
        }
    }
}