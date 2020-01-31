namespace WhyNotEarth.Meredith.Stripe.Data
{
    public class StripeOptions
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string PublishableKey { get; set; }

        public string RedirectUri { get; set; }

        public string EndpointSecret { get; set; }
    }
}