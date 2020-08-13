using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Hotel;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class RuleEntityConfig : IEntityTypeConfiguration<Rule>
    {
        public void Configure(EntityTypeBuilder<Rule> builder)
        {
            builder.ToTable("Rules", "ModuleHotel");
        }
    }
}