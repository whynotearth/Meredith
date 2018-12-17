namespace WhyNotEarth.Meredith.Stripe.Data
{
    using Newtonsoft.Json;

    public class StripeCredentialsModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("livemode")]
        public bool LiveMode { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("stripe_publishable_key")]
        public string StripePublishableKey { get; set; }

        [JsonProperty("stripe_user_id")]
        public string StripeUserId { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}