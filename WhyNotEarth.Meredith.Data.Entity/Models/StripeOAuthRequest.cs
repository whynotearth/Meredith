namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class StripeOAuthRequest : IEntityTypeConfiguration<StripeOAuthRequest>
    {
        public Company Company { get; set; }
        
        public Guid CompanyId { get; set; }
        
        public Guid Id { get; set; }
        
        public void Configure(EntityTypeBuilder<StripeOAuthRequest> builder)
        {
        }
    }
}