using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Hotel
{
    public class RuleTranslation
    {
        public int Id { get; set; }

        public int RuleId { get; set; }

        public Rule Rule { get; set; } = null!;

        public int LanguageId { get; set; }

        public Language Language { get; set; } = null!;

        public string? Text { get; set; }
    }
}