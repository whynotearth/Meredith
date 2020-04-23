using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class PageTranslation : IEntityTypeConfiguration<PageTranslation>
    {
        public int Id { get; set; }

        public Page Page { get; set; }

        public int PageId { get; set; }

        public Language Language { get; set; }

        public int LanguageId { get; set; }

        public string CallToAction { get; set; }

        public string Description { get; set; }

        public string Header { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public void Configure(EntityTypeBuilder<PageTranslation> builder)
        {
        }
    }
}
