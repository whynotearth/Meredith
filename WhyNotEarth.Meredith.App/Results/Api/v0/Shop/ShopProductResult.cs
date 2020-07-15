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

        public int DiscountPercent { get; }

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
            DiscountPercent = product.Category.Tenant.HasPromotion ? product.Category.Tenant.PromotionPercent : 0;
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

        public decimal Price { get; }

        public int DiscountPercent { get; }

        public VariationResult(Variation variation)
        {
            Name = variation.Name;
            Price = variation.Price.Amount;
            DiscountPercent = variation.Product.Category.Tenant.HasPromotion
                ? variation.Product.Category.Tenant.PromotionPercent
                : 0;
        }
    }

    public class ProductAttributeResult
    {
        public string Name { get; }

        public decimal Price { get; }

        public int DiscountPercent { get; }

        public ProductAttributeResult(ProductAttribute attribute)
        {
            Name = attribute.Name;
            Price = attribute.Price.Amount;
            DiscountPercent = attribute.Product.Category.Tenant.HasPromotion
                ? attribute.Product.Category.Tenant.PromotionPercent
                : 0;
        }
    }
}