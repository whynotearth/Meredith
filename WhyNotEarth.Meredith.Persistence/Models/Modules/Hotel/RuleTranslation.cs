using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class RuleTranslation
    {
        public int Id { get; set; }

        public int RuleId { get; set; }

        public Rule Rule { get; set; } = null!;

        public int LanguageId { get; set; }

        public Language Language { get; set; } = null!;

        public string? Text { get; set; }
    }

    public class RuleTranslationEntityConfig : IEntityTypeConfiguration<RuleTranslation>
    {
        public void Configure(EntityTypeBuilder<RuleTranslation> builder)
        {
            builder.ToTable("RuleTranslations", "ModuleHotel");
        }
    }
}