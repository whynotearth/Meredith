namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Page : IEntityTypeConfiguration<Page>
    {
        public string BackgroundImage { get; set; }

        public string CallToAction { get; set; }

        public string CallToActionLink { get; set; }

        public ICollection<Card> Cards { get; set; }

        public Company Company { get; set; }

        public Guid CompanyId { get; set; }

        public string Header { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Title { get; set; }

        public void Configure(EntityTypeBuilder<Page> builder)
        {

        }
    }
}