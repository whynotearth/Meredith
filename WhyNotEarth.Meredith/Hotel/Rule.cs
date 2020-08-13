using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Hotel
{
    public class Rule
    {
        public int Id { get; set; }

        public int HotelId { get; set; }

        public Hotel Hotel { get; set; } = null!;

        public ICollection<RuleTranslation>? Translations { get; set; }
    }
}