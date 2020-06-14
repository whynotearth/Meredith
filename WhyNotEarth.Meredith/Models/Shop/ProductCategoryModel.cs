using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Models
{
    public class ProductCategoryModel
    {
        [NotNull]
        [Mandatory]
        public string? TenantSlug { get; set; }

        [NotNull]
        [Mandatory]
        public string? Name { get; set; }

        public string? Description { get; set; }
        
        public string? ImageURL { get; set; }
    }
}
