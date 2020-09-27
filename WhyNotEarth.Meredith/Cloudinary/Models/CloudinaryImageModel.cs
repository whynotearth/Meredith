using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Cloudinary.Models
{
    public class CloudinaryImageModel
    {
        [NotNull]
        [Mandatory]
        public string? PublicId { get; set; }

        [NotNull]
        [Mandatory]
        public string? Url { get; set; }

        [NotNull]
        [Mandatory]
        public int? Width { get; set; }

        [NotNull]
        [Mandatory]
        public int? Height { get; set; }
        
        [NotNull]
        [Mandatory]
        public double FileSize { get; set; }
    }
}