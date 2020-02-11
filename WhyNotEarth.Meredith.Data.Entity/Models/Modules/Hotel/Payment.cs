namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Payment : IEntityTypeConfiguration<Payment>
    {
        public enum Statuses
        {
            IntentGenerated,
            Fulfilled
        }

        public decimal Amount { get; set; }

        public DateTime Created { get; set; }

        public int Id { get; set; }

        public string PaymentIntentId { get; set; }

        public Reservation Reservation { get; set; }

        public int ReservationId { get; set; }

        public Statuses Status { get; set; }

        public User User { get; set; }

        public int UserId { get; set; }

        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments", "ModuleHotel");
        }
    }
}