using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WhyNotEarth.Meredith.Public
{
    [DebuggerDisplay("{" + nameof(Slug) + "}")]
    public class Page
    {
        public int Id { get; set; }

        public string? Slug { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public int? TenantId { get; set; }

        public Tenant? Tenant { get; set; }

        public int? CategoryId { get; set; }

        public PageCategory? Category { get; set; }

        public Hotel.Hotel? Hotel { get; set; }

        public string? BackgroundImage { get; set; }

        public string? CallToActionLink { get; set; }

        public ICollection<Card>? Cards { get; set; }

        public string? Custom { get; set; }

        public string? FeaturedImage { get; set; }

        public ICollection<PageImage>? Images { get; set; }

        public string? LandingPageData { get; set; }

        public ICollection<Keyword>? Keywords { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? EditedAt { get; set; }

        public ICollection<PageTranslation>? Translations { get; set; }
    }

    public class PageImage : Image
    {
    }

    public class PageCategory : Category
    {
    }
}