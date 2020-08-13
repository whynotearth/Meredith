using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Hotel;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class SpaceTranslationEntityConfig : IEntityTypeConfiguration<SpaceTranslation>
    {
        public void Configure(EntityTypeBuilder<SpaceTranslation> builder)
        {
            builder.ToTable("SpaceTranslations", "ModuleHotel");
        }
    }
}