using System.Collections.Generic;
using System.Diagnostics;
using WhyNotEarth.Meredith.Stripe;

namespace WhyNotEarth.Meredith.Public
{
    [DebuggerDisplay("{" + nameof(Slug) + "}")]
    public class Company
    {
        public int Id { get; set; }

        public string Slug { get; set; } = null!;

        public string Name { get; set; } = null!;

        public ICollection<Page>? Pages { get; set; }

        public ICollection<Tenant>? Tenants { get; set; }

        public StripeAccount? StripeAccount { get; set; }
    }
}