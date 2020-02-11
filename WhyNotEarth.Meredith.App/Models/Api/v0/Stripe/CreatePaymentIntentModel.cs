namespace WhyNotEarth.Meredith.App.Models.Api.v0.Stripe
{
    using System.Collections.Generic;

    public class CreatePaymentIntentModel
    {
        public decimal Amount { get; set; }

        public int CompanyId { get; set; }

        public string Email { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}