using System;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class StripeOAuthRequest
    {
        public Guid Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;
    }
}