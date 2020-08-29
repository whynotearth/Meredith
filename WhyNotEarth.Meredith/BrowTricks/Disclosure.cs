namespace WhyNotEarth.Meredith.BrowTricks
{
    public class Disclosure
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public Public.Tenant Tenant { get; set; } = null!;

        public string Value { get; set; } = null!;
    }
}