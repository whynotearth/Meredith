using System;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Stripe
{
    public class StripeOAuthRequest
    {
        public Guid Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;
    }
}