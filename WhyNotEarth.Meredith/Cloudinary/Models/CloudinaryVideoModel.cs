using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Cloudinary.Models
{
    public class CloudinaryVideoModel
    {
        [NotNull]
        [Mandatory]
        public string? PublicId { get; set; }

        [NotNull]
        [Mandatory]
        public string? Url { get; set; }
    }
}