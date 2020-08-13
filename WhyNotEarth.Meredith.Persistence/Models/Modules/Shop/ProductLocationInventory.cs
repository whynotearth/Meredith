using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Shop
{
    public class ProductLocationInventoryEntityConfig : IEntityTypeConfiguration<ProductLocationInventory>
    {
        public void Configure(EntityTypeBuilder<ProductLocationInventory> builder)
        {
            builder.ToTable("ProductLocationInventories", "ModuleShop");
        }
    }
}