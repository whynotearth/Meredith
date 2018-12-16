namespace WhyNotEarth.Meredith.App.Models.Api.v0.Stripe
{
    using System;

    public class CreateModel
    {
        public decimal Amount { get; set; }
        
        public Guid CompanyId { get; set; }
        
        public string Token { get; set; }
    }
}