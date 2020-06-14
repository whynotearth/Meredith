using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class Product : IEntityTypeConfiguration<Product>
    {
        public int Id { get; set; }

        public int PageId { get; set; }

        public Page Page { get; set; }

        public int PriceId { get; set; }

        public Price Price { get; set; }
        
        public int CategoryId { get; set; }

        public ProductCategory Category { get; set; }

        public string Name { get; set; }

        public string ImageURL { get; set; }

        public List<Variation> Variations { get; set; }

        public List<ProductLocationInventory> ProductLocationInventories { get; set; }

        public List<ProductAttribute> ProductAttributes { get; set; }

        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "ModuleShop");
        }
    }
}