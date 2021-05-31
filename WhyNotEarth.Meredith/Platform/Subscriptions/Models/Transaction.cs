using System;

namespace WhyNotEarth.Meredith.Platform.Subscriptions.Models
{
    public class Transaction
    {
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string TransactionId { get; set; } = null!;

        public string PaymentMethod { get; set; } = null!;

        public string? StatementLink { get; set; }
    }
}