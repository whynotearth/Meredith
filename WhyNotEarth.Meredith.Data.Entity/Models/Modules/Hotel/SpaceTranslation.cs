using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    public class SpaceTranslation : IEntityTypeConfiguration<SpaceTranslation>
    {
        public int Id { get; set; }

        public Space Space { get; set; }

        public int SpaceId { get; set; }

        public Language Language { get; set; }

        public int LanguageId { get; set; }

        public string Name { get; set; }

        public void Configure(EntityTypeBuilder<SpaceTranslation> builder)
        {
            builder.ToTable("SpaceTranslations", "ModuleHotel");
        }
    }
}