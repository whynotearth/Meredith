using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Shop
{
    public class ProductCategoryResult
    {
        public int Id { get; }

        public string Name { get; }

        public string? Description { get; }

        public string? ImageUrl { get; }

        public ProductCategoryResult(ProductCategory productCategory)
        {
            Id = productCategory.Id;
            Name = productCategory.Name;
            Description = productCategory.Description;
            ImageUrl = productCategory.Image?.Url;
        }
    }
}