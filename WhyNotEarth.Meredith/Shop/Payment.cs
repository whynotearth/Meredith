using System;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Shop
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
}