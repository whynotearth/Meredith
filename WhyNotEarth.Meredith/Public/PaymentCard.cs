namespace WhyNotEarth.Meredith.Public
{
    public class PaymentCard
    {
        public enum Brands
        {
            Unknown,
            Amex,
            DinersClub,
            Discover,
            Jcb,
            Mastercard,
            Visa,
            UnionPay,
        }

        public int Id { get; set; }

        public string StripeId { get; set; } = null!;

        public Customer? Customer { get; set; } = null!;

        public int CustomerId { get; set; }

        public Brands Brand { get; set; }

        public string Last4 { get; set; } = null!;

        public byte ExpirationMonth { get; set; }

        public ushort ExpirationYear { get; set; }
    }
}