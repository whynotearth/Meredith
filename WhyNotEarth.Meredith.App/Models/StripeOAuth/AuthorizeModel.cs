using Newtonsoft.Json;

namespace WhyNotEarth.Meredith.App.Models.StripeOAuth
{
    public class AuthorizeModel
    {
        [JsonProperty("code")]
        public string Code { get; set; } = null!;

        [JsonProperty("error")]
        public string Error { get; set; } = null!;

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; } = null!;

        [JsonProperty("state")]
        public string State { get; set; } = null!;
    }
}