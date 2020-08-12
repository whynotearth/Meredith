using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Shop
{
    public class Product
    {
        public int Id { get; set; }

        public int PageId { get; set; }

        public Page Page { get; set; } = null!;

        public int PriceId { get; set; }

        public Price Price { get; set; } = null!;

        public int CategoryId { get; set; }

        public ProductCategory Category { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsAvailable { get; set; }

        public ProductImage? Image { get; set; }

        public List<Variation>? Variations { get; set; }

        public List<ProductLocationInventory>? ProductLocationInventories { get; set; }

        public List<ProductAttribute>? ProductAttributes { get; set; }
    }

    public class ProductEntityConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "ModuleShop");
        }
    }
}