namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;

    public class Page : IEntityTypeConfiguration<Page>
    {
        public string BackgroundImage { get; set; }

        public string CallToAction { get; set; }

        public string CallToActionLink { get; set; }

        public ICollection<Card> Cards { get; set; }

        public Category Category { get; set; }

        public int? CategoryId { get; set; }

        public Company Company { get; set; }

        public int CompanyId { get; set; }

        public string Custom { get; set; }

        public string Description { get; set; }

        public string FeaturedImage { get; set; }

        public string Header { get; set; }

        public Hotel Hotel { get; set; }

        public int Id { get; set; }

        public ICollection<Image> Images { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Title { get; set; }

        public void Configure(EntityTypeBuilder<Page> builder)
        {
            builder.Property(e => e.Custom).HasColumnType("jsonb");
        }
    }
}