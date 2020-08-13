namespace WhyNotEarth.Meredith.Public
{
    public class Card
    {
        public enum CardTypes
        {
            Card
        }

        public int Id { get; set; }

        public string BackgroundUrl { get; set; } = null!;

        public string CallToAction { get; set; } = null!;

        public string? CallToActionUrl { get; set; }

        public int PageId { get; set; }

        public Page Page { get; set; } = null!;

        public string? PosterUrl { get; set; }

        public CardTypes CardType { get; set; }

        public int Order { get; set; }

        public string Text { get; set; } = null!;
    }
}