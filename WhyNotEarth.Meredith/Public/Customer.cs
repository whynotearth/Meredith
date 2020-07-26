namespace WhyNotEarth.Meredith.Public
{
    public class Customer
    {
        public int Id { get; set; }

        public string StripeId { get; set; } = null!;

        public Tenant Tenant { get; set; } = null!;

        public int TenantId { get; set; }
    }
}