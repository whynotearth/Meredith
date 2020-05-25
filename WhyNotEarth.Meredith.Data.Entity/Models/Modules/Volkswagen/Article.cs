using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen
{
    public class Article : IEntityTypeConfiguration<Article>
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int CategoryId { get; set; }

        public ArticleCategory Category { get; set; }

        public string Headline  { get; set; }

        public string Description { get; set; }

        public string ImageCaption { get; set; }

        public DateTime? EventDate { get; set; }

        public int? ImageId { get; set; }

        public ArticleImage Image { get; set; }

        public int? Order { get; set; }

        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("Articles", "ModuleVolkswagen");
            builder.Property(e => e.Date).HasColumnType("date");
            builder.Property(e => e.EventDate).HasColumnType("date");
        }
    }

    public class ArticleImage : Image
    {
    }

    public class ArticleCategory : Category, IEntityTypeConfiguration<ArticleCategory>
    {
        public string Color { get; set; }

        public int Priority { get; set; }

        public void Configure(EntityTypeBuilder<ArticleCategory> builder)
        {
            builder.Property(b => b.Color).IsRequired();
        }
    }
}