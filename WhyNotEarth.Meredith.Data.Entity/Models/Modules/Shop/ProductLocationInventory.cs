using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class ProductLocationInventory : IEntityTypeConfiguration<ProductLocationInventory>
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int LocationId { get; set; }
        
        public Location Location { get; set; }

        public int Count { get; set; }

        public void Configure(EntityTypeBuilder<ProductLocationInventory> builder)
        {
            builder.ToTable("ProductLocationInventories", "ModuleShop");
        }
    }
}