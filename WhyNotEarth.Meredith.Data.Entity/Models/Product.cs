using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Product : IEntityTypeConfiguration<Product>
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<ProductImage> Images { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public Tenant Tenant { get; set; }

        public ProductCategory Category { get; set; }

        public void Configure(EntityTypeBuilder<Product> builder)
        {
        }
    }

    public class ProductCategory : Category
    {
        public int TenantId { get; set; }

        public Tenant Tenant { get; set; }

        public string ImageURL { get; set; }
    }

    public class ProductImage : Image
    {
    }
}