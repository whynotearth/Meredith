using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Shop
{
    public class ShopProductResult
    {
        public int Id { get; }

        public int PageId { get; }

        public int PriceId { get; }

        public List<ProductLocationInventoryResult> ProductLocationInventories { get; }

        public List<VariationResult> Variations { get; }

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
        public int Id { get; }

        public int LocationId { get; }

        public int Count { get; }

        public ProductLocationInventoryResult(ProductLocationInventory productLocationInventory)
        {
            Id = productLocationInventory.Id;
            LocationId = productLocationInventory.LocationId;
            Count = productLocationInventory.Count;
        }
    }

    public class VariationResult
    {
        public int Id { get; }

        public string Name { get; }

        public VariationResult(Variation variation)
        {
            Id = variation.Id;
            Name = variation.Name;
        }
    }
}