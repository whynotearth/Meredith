using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks
{
    public class PmuQuestion
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;

        public string Question { get; set; } = null!;
    }

    public class PmuQuestionEntityConfig : IEntityTypeConfiguration<PmuQuestion>
    {
        public void Configure(EntityTypeBuilder<PmuQuestion> builder)
        {
            builder.ToTable("PmuQuestions", "ModuleBrowTricks");
        }
    }
}