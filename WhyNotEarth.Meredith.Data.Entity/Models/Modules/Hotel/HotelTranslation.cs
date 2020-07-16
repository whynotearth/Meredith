using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    public class HotelTranslation
    {
        public int Id { get; set; }

        public int HotelId { get; set; }

        public Hotel Hotel { get; set; } = null!;

        public int LanguageId { get; set; }

        public Language Language { get; set; } = null!;

        public string? GettingAround { get; set; }

        public string? Location { get; set; }
    }

    public class HotelTranslationEntityConfig : IEntityTypeConfiguration<HotelTranslation>
    {
        public void Configure(EntityTypeBuilder<HotelTranslation> builder)
        {
            builder.ToTable("HotelTranslations", "ModuleHotel");
        }
    }
}