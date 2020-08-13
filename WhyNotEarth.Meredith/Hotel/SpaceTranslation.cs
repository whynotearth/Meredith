using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Hotel
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
}