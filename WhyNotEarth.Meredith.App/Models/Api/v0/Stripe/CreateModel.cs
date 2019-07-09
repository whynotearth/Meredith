namespace WhyNotEarth.Meredith.App.Models.Api.v0.Stripe
{
    using System.Collections.Generic;

    public class CreateModel
    {
        public decimal Amount { get; set; }

        public int CompanyId { get; set; }

        public string Email { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public string Token { get; set; }
    }
}