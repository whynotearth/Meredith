using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Hotel
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
}