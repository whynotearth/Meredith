namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Platform
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using WhyNotEarth.Meredith.Public;

    public class SubscriptionConfig : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.ToTable("Subscriptions", "Platform");
        }
    }
}