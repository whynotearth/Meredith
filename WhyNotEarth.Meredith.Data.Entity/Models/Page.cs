using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    [DebuggerDisplay("{" + nameof(Slug) + "}")]
    public class Page : IEntityTypeConfiguration<Page>
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public int? TenantId { get; set; }

        public Tenant Tenant { get; set; }

        public int? CategoryId { get; set; }

        public PageCategory Category { get; set; }

        public Hotel Hotel { get; set; }

        public string BackgroundImage { get; set; }

        public string CallToAction { get; set; }

        public string CallToActionLink { get; set; }

        public ICollection<Card> Cards { get; set; }

        public string Custom { get; set; }

        public string Description { get; set; }

        public string FeaturedImage { get; set; }

        public string Header { get; set; }

        public ICollection<PageImage> Images { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Title { get; set; }

        public string LandingPageData { get; set; }

        public ICollection<Keyword> Keywords { get; set; }

        public DateTime CreationDateTime { get; set; }
                
        public DateTime? EditDateTime { get; set; }
        
        public void Configure(EntityTypeBuilder<Page> builder)
        {
            builder.Property(e => e.Custom).HasColumnType("jsonb");
        }
    }

    public class PageImage : Image
    {
    }

    public class PageCategory : Category
    {
    }
}