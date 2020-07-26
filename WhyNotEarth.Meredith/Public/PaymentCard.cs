namespace WhyNotEarth.Meredith.Public
{
    public class PaymentCard
    {
        public int Id { get; set; }

        public string StripeId { get; set; } = null!;

        public Customer? Customer { get; set; } = null!;

        public int CustomerId { get; set; }
    }
}