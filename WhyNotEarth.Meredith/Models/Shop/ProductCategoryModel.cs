using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Models.Shop
{
    public class ProductCategoryModel
    {
        [Mandatory]
        public string? TenantSlug { get; set; }

        [Mandatory]
        public string? Slug { get; set; }

        [Mandatory]
        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}
