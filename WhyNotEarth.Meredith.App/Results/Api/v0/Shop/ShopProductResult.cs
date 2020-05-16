using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Shop
{
    /* I coudln't use ProductResult as the name of the class 
     * because there is another one in Public module and 
     * swagger can not document them both.
     */
    public class ShopProductResult
    {
        public int Id { get; set; }

        public int PageId { get; set; }

        public int PriceId { get; set; }

        public List<ProductLocationInventoryResult> ProductLocationInventories { get; set; } = null!;

        public List<VariationResult> Variations { get; set; } = null!;

        public ShopProductResult()
        {
        }

        public ShopProductResult(Product product)
        {
            Id = product.Id;
            PageId = product.PageId;
            PriceId = product.PriceId;
            ProductLocationInventories = product.ProductLocationInventories
                .Select(item => new ProductLocationInventoryResult(item))
                .ToList();
            Variations = product.Variations
                .Select(item => new VariationResult(item))
                .ToList();
        }
    }

    public class ProductLocationInventoryResult
    {
        public int Id { get; set; }

        public int LocationId { get; set; }

        public int Count { get; set; }

        public ProductLocationInventoryResult(ProductLocationInventory productLocationInventory)
        {
            Id = productLocationInventory.Id;
            LocationId = productLocationInventory.LocationId;
            Count = productLocationInventory.Count;
        }
    }

    public class VariationResult
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public VariationResult(Variation variation)
        {
            Id = variation.Id;
            Name = variation.Name;
        }
    }
}
