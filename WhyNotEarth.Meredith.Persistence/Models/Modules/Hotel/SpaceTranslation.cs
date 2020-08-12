using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class SpaceTranslation
    {
        public int Id { get; set; }

        public int SpaceId { get; set; }

        public Space Space { get; set; } = null!;

        public int LanguageId { get; set; }

        public Language Language { get; set; } = null!;

        public string? Name { get; set; }
    }

    public class SpaceTranslationEntityConfig : IEntityTypeConfiguration<SpaceTranslation>
    {
        public void Configure(EntityTypeBuilder<SpaceTranslation> builder)
        {
            builder.ToTable("SpaceTranslations", "ModuleHotel");
        }
    }
}