using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.BrowTricks
{
    public class Disclosure
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; } = null!;

        public string Value { get; set; } = null!;
    }

    public class DisclosureEntityConfig : IEntityTypeConfiguration<Disclosure>
    {
        public void Configure(EntityTypeBuilder<Disclosure> builder)
        {
            builder.ToTable("Disclosures", "ModuleBrowTricks");
        }
    }
}