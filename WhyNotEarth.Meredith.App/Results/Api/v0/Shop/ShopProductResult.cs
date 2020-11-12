using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Shop
{
    public class ShopProductResult
    {
        public int Id { get; }

        public int PageId { get; }

        public decimal OriginalPrice { get; }

        public int DiscountPercent { get; }

        public decimal Price { get; }

        public ProductCategoryResult Category { get; }

        public string Name { get; }

        public string? Description { get; }

        public bool IsAvailable { get; }

        public string? ImageUrl { get; }

        public List<ProductLocationInventoryResult>? LocationInventories { get; }

        public List<VariationResult>? Variations { get; }

        public List<ProductAttributeResult>? Attributes { get; }

        public ShopProductResult(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            PageId = product.PageId;
            OriginalPrice = product.Price.Amount;
            DiscountPercent = product.Category.Tenant.HasPromotion ? product.Category.Tenant.PromotionPercent : 0;

            var discount = DiscountPercent / 100M;
            Price = OriginalPrice - (OriginalPrice * discount);

            Category = new ProductCategoryResult(product.Category);
            Description = product.Description;
            IsAvailable = product.IsAvailable;
            ImageUrl = product.Image?.Url;

            LocationInventories = product.ProductLocationInventories
                ?.Select(item => new ProductLocationInventoryResult(item))
                .ToList();
            Variations = product.Variations
                ?.Select(item => new VariationResult(item))
                .ToList();
            Attributes = product.ProductAttributes
                ?.Select(item => new ProductAttributeResult(item))
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

        public decimal Price { get; }

        public int DiscountPercent { get; }

        public VariationResult(Variation variation)
        {
            Id = variation.Id;
            Name = variation.Name;
            Price = variation.Price.Amount;
            DiscountPercent = variation.Product.Category.Tenant.HasPromotion
                ? variation.Product.Category.Tenant.PromotionPercent
                : 0;
        }
    }

    public class ProductAttributeResult
    {
        public int Id { get; }

        public string Name { get; }

        public decimal Price { get; }

        public int DiscountPercent { get; }

        public ProductAttributeResult(ProductAttribute attribute)
        {
            Id = attribute.Id;
            Name = attribute.Name;
            Price = attribute.Price.Amount;
            DiscountPercent = attribute.Product.Category.Tenant.HasPromotion
                ? attribute.Product.Category.Tenant.PromotionPercent
                : 0;
        }
    }
}