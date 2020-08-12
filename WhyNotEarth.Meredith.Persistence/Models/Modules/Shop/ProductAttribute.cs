using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Shop
{
    public class ProductAttribute
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int PriceId { get; set; }

        public Price Price { get; set; } = null!;
    }

    public class ProductAttributeEntityConfig : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.ToTable("ProductAttributes", "ModuleShop");
        }
    }
}
