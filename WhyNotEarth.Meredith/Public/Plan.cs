namespace WhyNotEarth.Meredith.Public
{
    public class Plan
    {
        public int Id { get; set; }

        public string StripeId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public Platform Platform { get; set; } = null!;

        public int PlatformId { get; set; }

        public decimal Price { get; set; }
    }
}