using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Shop
{
    public class VariationEntityConfig : IEntityTypeConfiguration<Variation>
    {
        public void Configure(EntityTypeBuilder<Variation> builder)
        {
            builder.ToTable("Variations", "ModuleShop");
            builder.Property(b => b.Name).IsRequired();
        }
    }
}