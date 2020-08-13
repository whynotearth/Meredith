using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Hotel;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class AmenityTranslationEntityConfig : IEntityTypeConfiguration<AmenityTranslation>
    {
        public void Configure(EntityTypeBuilder<AmenityTranslation> builder)
        {
            builder.ToTable("AmenityTranslations", "ModuleHotel");
        }
    }
}