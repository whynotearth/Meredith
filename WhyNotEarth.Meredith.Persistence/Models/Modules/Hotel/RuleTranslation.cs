using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Hotel;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class RuleTranslationEntityConfig : IEntityTypeConfiguration<RuleTranslation>
    {
        public void Configure(EntityTypeBuilder<RuleTranslation> builder)
        {
            builder.ToTable("RuleTranslations", "ModuleHotel");
        }
    }
}