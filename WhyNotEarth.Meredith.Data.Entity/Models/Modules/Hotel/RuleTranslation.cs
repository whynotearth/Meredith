using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    public class RuleTranslation : IEntityTypeConfiguration<RuleTranslation>
    {
        public int Id { get; set; }

        public Rule Rule { get; set; }

        public int RuleId { get; set; }

        public Language Language { get; set; }

        public int LanguageId { get; set; }

        public string Text { get; set; }

        public void Configure(EntityTypeBuilder<RuleTranslation> builder)
        {
            builder.ToTable("RuleTranslations", "ModuleHotel");
        }
    }
}