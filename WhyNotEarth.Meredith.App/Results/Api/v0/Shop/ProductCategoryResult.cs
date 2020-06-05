using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Shop
{
    public class ProductCategoryResult
    {
        public int Id { get; }

        public string? Name { get; }

        public string? Image { get; }

        public string? Description { get; }

        public string TenantSlug { get; }

        public ProductCategoryResult(ProductCategory productCategory)
        {
            this.Description = productCategory.Description;
            this.Id = productCategory.Id;
            this.Name = productCategory.Name;
            this.Image = productCategory.Image?.Url;
            this.TenantSlug = productCategory.Tenant.Slug;
        }
    }
}
