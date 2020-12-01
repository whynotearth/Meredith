using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Stripe;

namespace WhyNotEarth.Meredith.Persistence.Models
{
    public class StripeAccountEntityConfig : IEntityTypeConfiguration<StripeAccount>
    {
        public void Configure(EntityTypeBuilder<StripeAccount> builder)
        {
        }
    }
}