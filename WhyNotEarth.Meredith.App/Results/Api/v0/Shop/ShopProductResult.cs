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

        public ProductCategoryResult Category { get; }

        public string Name { get; }

        public string? Description { get; }

        public bool IsAvailable { get; }

        public string? ImageUrl { get; }

        public List<ProductLocationInventoryResult> LocationInventories { get; }

        public List<VariationResult> Variations { get; }

        public List<ProductAttributeResult> Attributes { get; }

        public ShopProductResult(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            PageId = product.PageId;
            Price = product.Price.Amount;
            Category = new ProductCategoryResult(product.Category);
            Description = product.Description;
            IsAvailable = product.IsAvailable;
            ImageUrl = product.Image?.Url;

            LocationInventories = product.ProductLocationInventories
                .Select(item => new ProductLocationInventoryResult(item))
                .ToList();
            Variations = product.Variations
                .Select(item => new VariationResult(item))
                .ToList();
            Attributes = product.ProductAttributes
                .Select(item => new ProductAttributeResult(item))
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

        public decimal Price { get; set; }

        public VariationResult(Variation variation)
        {
            Id = variation.Id;
            Name = variation.Name;
            Price = variation.Price.Amount;
        }
    }

    public class ProductAttributeResult
    {
        public int Id { get; }

        public string Name { get; }

        public decimal Price { get; set; }

        public ProductAttributeResult(ProductAttribute attribute)
        {
            Id = attribute.Id;
            Name = attribute.Name;
            Price = attribute.Price.Amount;
        }
    }
}