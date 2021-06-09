namespace WhyNotEarth.Meredith.Public
{
    public class Subscription
    {
        public enum SubscriptionStatuses
        {
            Unknown = 0,
            Active,
            Cancelled
        }

        public int Id { get; set; }

        public Customer Customer { get; set; } = null!;

        public int CustomerId { get; set; }

        public PaymentCard? Card { get; set; }

        public int? CardId { get; set; }

        public string StripeId { get; set; } = null!;

        public Plan Plan { get; set; } = null!;

        public int PlanId { get; set; }

        public SubscriptionStatuses Status { get; set; }
    }
}