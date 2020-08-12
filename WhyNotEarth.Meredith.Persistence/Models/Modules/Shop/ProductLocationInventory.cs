using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class ProductLocationInventory
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public int LocationId { get; set; }

        public Location Location { get; set; } = null!;

        public int Count { get; set; }
    }

    public class ProductLocationInventoryEntityConfig : IEntityTypeConfiguration<ProductLocationInventory>
    {
        public void Configure(EntityTypeBuilder<ProductLocationInventory> builder)
        {
            builder.ToTable("ProductLocationInventories", "ModuleShop");
        }
    }
}