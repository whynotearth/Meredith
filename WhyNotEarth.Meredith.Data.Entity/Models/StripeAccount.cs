#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class StripeAccount
    {
        public int Id { get; set; }
        
        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public string AccessToken { get; set; } = null!;

        public bool LiveMode { get; set; }

        public string RefreshToken { get; set; } = null!;

        public string Scope { get; set; } = null!;

        public string StripePublishableKey { get; set; } = null!;

        public string StripeUserId { get; set; } = null!;

        public string TokenType { get; set; } = null!;
    }

    public class StripeAccountEntityConfig : IEntityTypeConfiguration<StripeAccount>
    {
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