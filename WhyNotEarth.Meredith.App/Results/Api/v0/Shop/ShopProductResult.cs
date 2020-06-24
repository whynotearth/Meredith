using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Shop
{
    public class ShopProductResult
    {
        public int Id { get; }

        public int PageId { get; }

        public decimal Price { get; }

        public int CategoryId { get; }

        public string Name { get; }

        public string? Description { get; }

        public List<ProductLocationInventoryResult> ProductLocationInventories { get; }

        public List<VariationResult> Variations { get; }

        public List<ProductAttributeResult> ProductAttributes { get; }

        public ShopProductResult(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            PageId = product.PageId;
            Price = product.Price.Amount;
            CategoryId = product.CategoryId;
            Description = product.Description;

            ProductLocationInventories = product.ProductLocationInventories
                .Select(item => new ProductLocationInventoryResult(item))
                .ToList();
            Variations = product.Variations
                .Select(item => new VariationResult(item))
                .ToList();
            ProductAttributes = product.ProductAttributes
                .Select(item => new ProductAttributeResult(item))
                .ToList();
        }
    }

    public class ProductLocationInventoryResult
    {
        public int LocationId { get; }

        public int Count { get; }

        public ProductLocationInventoryResult(ProductLocationInventory productLocationInventory)
        {
            LocationId = productLocationInventory.LocationId;
            Count = productLocationInventory.Count;
        }
    }

    public class VariationResult
    {
        public string Name { get; }

        public decimal Price { get; set; }

        public VariationResult(Variation variation)
        {
            Name = variation.Name;
            Price = variation.Price.Amount;
        }
    }

    public class ProductAttributeResult
    {
        public string Name { get; }

        public decimal Price { get; set; }

        public ProductAttributeResult(ProductAttribute variation)
        {
            Name = variation.Name;
            Price = variation.Price.Amount;
        }
    }
}