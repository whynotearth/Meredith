using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class AmenityTranslation
    {
        public int Id { get; set; }

        public int AmenityId { get; set; }

        public Amenity Amenity { get; set; } = null!;

        public int LanguageId { get; set; }

        public Language Language { get; set; } = null!;

        public string? Text { get; set; }
    }

    public class AmenityTranslationEntityConfig : IEntityTypeConfiguration<AmenityTranslation>
    {
        public void Configure(EntityTypeBuilder<AmenityTranslation> builder)
        {
            builder.ToTable("AmenityTranslations", "ModuleHotel");
        }
    }
}