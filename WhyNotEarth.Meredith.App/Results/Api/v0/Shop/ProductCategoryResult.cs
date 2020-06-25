using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Shop
{
    public class ProductCategoryResult
    {
        public int Id { get; }

        public string TenantSlug { get; }

        public string Name { get; }

        public string? Description { get; }

        public string? ImageUrl { get; }

        public ProductCategoryResult(ProductCategory productCategory)
        {
            Id = productCategory.Id;
            TenantSlug = productCategory.Tenant.Slug;
            Name = productCategory.Name;
            Description = productCategory.Description;
            ImageUrl = productCategory.Image?.Url;
        }
    }
}