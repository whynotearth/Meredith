namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using System;

    public class StripeAccount
    {
        public Guid Id { get; set; }
        
        public Company Company { get; set; }
        
        public Guid CompanyId { get; set; }
    }
}