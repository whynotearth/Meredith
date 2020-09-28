using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Stripe
{
    public class StripeAccount
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public string AccessToken { get; set; } = null!;

        public bool LiveMode { get; set; }

        public string RefreshToken { get; set; } = null!;

        public string Scope { get; set; } = null!;

        public string StripePublishableKey { get; set; } = null!;

        public string StripeUserId { get; set; } = null!;

        public string TokenType { get; set; } = null!;
    }
}