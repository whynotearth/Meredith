using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Cloudinary.Models;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class BrowTricksImageModel
    {
        public int? ClientId { get; set; }

        [NotNull]
        [Mandatory]
        public CloudinaryImageModel? Image { get; set; }

        public string? Description { get; set; }
    }
}