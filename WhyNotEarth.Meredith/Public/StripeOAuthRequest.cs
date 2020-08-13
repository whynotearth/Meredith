using System;

namespace WhyNotEarth.Meredith.Public
{
    public class StripeOAuthRequest
    {
        public Guid Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;
    }
}