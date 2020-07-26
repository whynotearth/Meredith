namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Platform
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using WhyNotEarth.Meredith.Public;

    public class CardConfig : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.ToTable("Cards", "Platform");
        }
    }
}