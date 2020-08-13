using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Hotel;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class HotelTranslationEntityConfig : IEntityTypeConfiguration<HotelTranslation>
    {
        public void Configure(EntityTypeBuilder<HotelTranslation> builder)
        {
            builder.ToTable("HotelTranslations", "ModuleHotel");
        }
    }
}