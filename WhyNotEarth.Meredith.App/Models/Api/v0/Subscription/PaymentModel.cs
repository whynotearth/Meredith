namespace WhyNotEarth.Meredith.App.Models.Api.v0.Subscription
{
    using System;

    public class PaymentModel
    {
        public DateTime PaymentDate { get; set; }

        public decimal Total { get; set; }

        public string TransactionId { get; set; } = null!;

        public string PaymentMethod { get; set; } = null!;

        public string? StatementLink { get; set; }
    }
}