namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Platform
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using WhyNotEarth.Meredith.Public;

    public class CardConfig : IEntityTypeConfiguration<PaymentCard>
    {
        public void Configure(EntityTypeBuilder<PaymentCard> builder)
        {
            builder.ToTable("Cards", "Platform");
        }
    }
}