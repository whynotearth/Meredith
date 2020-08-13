using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.BrowTricks
{
    public class DisclosureEntityConfig : IEntityTypeConfiguration<Disclosure>
    {
        public void Configure(EntityTypeBuilder<Disclosure> builder)
        {
            builder.ToTable("Disclosures", "ModuleBrowTricks");
        }
    }
}