namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class HotelTranslation : IEntityTypeConfiguration<HotelTranslation>
    {
        public int Id { get; set; }

        public Hotel Hotel { get; set; }

        public int HotelId { get; set; }

        public Language Language { get; set; }

        public int LanguageId { get; set; }

        public string GettingAround { get; set; }

        public string Location { get; set; }

        public void Configure(EntityTypeBuilder<HotelTranslation> builder)
        {
            builder.ToTable("HotelTranslations", "ModuleHotel");
        }
    }
}