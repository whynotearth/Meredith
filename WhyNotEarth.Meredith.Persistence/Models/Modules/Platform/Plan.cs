using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Platform
{

    public class PlanConfig : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.ToTable("Plans", "Platform");
        }
    }
}