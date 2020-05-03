using Newtonsoft.Json;

namespace WhyNotEarth.Meredith.Stripe.Data
{
    public class StripeCredentialsModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = null!;

        [JsonProperty("livemode")]
        public bool LiveMode { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = null!;

        [JsonProperty("scope")]
        public string Scope { get; set; } = null!;

        [JsonProperty("stripe_publishable_key")]
        public string StripePublishableKey { get; set; } = null!;

        [JsonProperty("stripe_user_id")]
        public string StripeUserId { get; set; } = null!;

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = null!;
    }
}