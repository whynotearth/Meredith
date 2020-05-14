using System.Collections.Generic;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Public
{
    public class ProductModel
    {
        public int TenantId { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public string Currency { get; set; } = null!;

        public ICollection<ProductImage>? Images { get; set; }
    }
}
