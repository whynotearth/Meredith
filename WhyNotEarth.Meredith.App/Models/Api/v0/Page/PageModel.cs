using System.Collections.Generic;

namespace WhyNotEarth.Meredith.App.Models.Api.V0.Page
{
    public class PageModel
    {
        public int Id { get; set; }

        public string Brand { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string H2 { get; set; }

        public string BackgroundImage { get; set; }

        public List<Image> Images { get; set; } = new List<Image>();

        public string CtaText { get; set; }

        public string CtaLink { get; set; }

        public List<Story> Stories { get; set; } = new List<Story>();

        public object Custom { get; set; }

        public Dictionary<string, object> Modules { get; set; } = new Dictionary<string, object>();

        public string FeaturedImage { get; set; }

        public string Slug { get; set; }
    }

    public class Story
    {
        public string Content { get; set; }

        public string CtaText { get; set; }

        public string CtaLink { get; set; }

        public string Image { get; set; }

        public string Blur { get; set; }

        public string Type { get; set; }
    }

    public class Image
    {
        public int Order { get; set; }

        public string Url { get; set; }

    }
}