namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class StripeAccount : IEntityTypeConfiguration<StripeAccount>
    {
        public string AccessToken { get; set; }

        public Company Company { get; set; }

        public int CompanyId { get; set; }

        public int Id { get; set; }

        public bool LiveMode { get; set; }

        public string RefreshToken { get; set; }

        public string Scope { get; set; }

        public string StripePublishableKey { get; set; }

        public string StripeUserId { get; set; }

        public string TokenType { get; set; }

        public void Configure(EntityTypeBuilder<StripeAccount> builder)
        {
            builder.Property(e => e.AccessToken).HasMaxLength(64);
            builder.Property(e => e.RefreshToken).HasMaxLength(64);
            builder.Property(e => e.Scope).HasMaxLength(32);
            builder.Property(e => e.StripePublishableKey).HasMaxLength(64);
            builder.Property(e => e.StripeUserId).HasMaxLength(64);
            builder.Property(e => e.TokenType).HasMaxLength(64);
        }
    }
}