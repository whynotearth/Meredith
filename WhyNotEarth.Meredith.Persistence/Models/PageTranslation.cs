namespace WhyNotEarth.Meredith.Persistence.Models
{
    public class PageTranslation
    {
        public int Id { get; set; }

        public int PageId { get; set; }

        public Page Page { get; set; } = null!;

        public int LanguageId { get; set; }

        public Language Language { get; set; } = null!;

        public string? CallToAction { get; set; }

        public string? Description { get; set; }

        public string? Header { get; set; }

        public string? Name { get; set; }

        public string? Title { get; set; }
    }
}
