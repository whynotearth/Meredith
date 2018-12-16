namespace WhyNotEarth.Meredith.App.Areas.Stripe.StripeOAuth
{
    using Newtonsoft.Json;

    public class AuthorizeModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }
}