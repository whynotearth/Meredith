#nullable enable

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Image
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? AltText { get; set; }

        public int Order { get; set; }

        public string Url { get; set; } = null!;

        public int? Width { get; set; }

        public int? Height { get; set; }
    }
}