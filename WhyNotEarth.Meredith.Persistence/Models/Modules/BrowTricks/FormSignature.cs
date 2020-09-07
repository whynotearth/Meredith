using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.BrowTricks
{
    public class FormSignatureEntityConfig : IEntityTypeConfiguration<FormSignature>
    {
        public void Configure(EntityTypeBuilder<FormSignature> builder)
        {
            builder.ToTable("FormSignatures", "ModuleBrowTricks");
        }
    }
}