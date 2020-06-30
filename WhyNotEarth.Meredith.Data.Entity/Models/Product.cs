#nullable enable

using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Product
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;

        public int CategoryId { get; set; }

        public ProductCategory Category { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public ICollection<ProductImage>? Images { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; } = null!;
    }

    public class ProductCategory : Category
    {
        public int TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
    }

    public class ProductImage : Image
    {
    }
}