using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Persistence.Models
{
    public class Language
    {
        public int Id { get; set; }

        public string Culture { get; set; } = null!;

        public string Name { get; set; } = null!;
    }

    public class LanguageEntityConfig : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Languages", "public");
        }
    }
}
