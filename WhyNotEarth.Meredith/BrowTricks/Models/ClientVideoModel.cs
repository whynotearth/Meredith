using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Cloudinary.Models;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class ClientVideoModel
    {
        [NotNull]
        [Mandatory]
        public int? ClientId { get; set; }

        [NotNull]
        [Mandatory]
        public CloudinaryVideoModel? Video { get; set; }

        public string? Description { get; set; }
    }
}