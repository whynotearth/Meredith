using System.Collections.Generic;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Shop
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
}