using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Stripe;

namespace WhyNotEarth.Meredith.Persistence.Models
{
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