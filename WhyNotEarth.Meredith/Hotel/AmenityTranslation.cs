using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Hotel
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
}