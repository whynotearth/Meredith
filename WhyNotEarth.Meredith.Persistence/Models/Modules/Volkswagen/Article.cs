using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Volkswagen
{
    public class ArticleEntityConfig : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("Articles", "ModuleVolkswagen");
            builder.Property(e => e.Date).HasColumnType("date");
            builder.Property(e => e.EventDate).HasColumnType("date");
        }
    }

    public class ArticleCategoryEntityConfig : CategoryEntityConfig
    {
        public void Configure(EntityTypeBuilder<ArticleCategory> builder)
        {
            builder.Property(b => b.Color).IsRequired();
        }
    }
}