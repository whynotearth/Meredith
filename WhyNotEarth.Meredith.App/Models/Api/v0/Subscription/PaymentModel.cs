namespace WhyNotEarth.Meredith.App.Models.Api.v0.Subscription
{
    using System;

    public class PaymentModel
    {
        public string Subscription { get; set; } = null!;

        public DateTime PaymentDate { get; set; }

        public decimal SubscriptionAmount { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Total { get; set; }

        public string TransactionId { get; set; } = null!;

        public string PaymentMethod { get; set; } = null!;
    }
}