namespace WhyNotEarth.Meredith.Stripe.Data
{
    public class StripeOptions
    {
        public string ClientId { get; set; } = null!;

        public string ClientSecret { get; set; } = null!;

        public string PublishableKey { get; set; } = null!;

        public string RedirectUri { get; set; } = null!;

        public string EndpointSecret { get; set; } = null!;
    }
}