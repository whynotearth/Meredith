using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Cloudinary.Models;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class ClientModel
    {
        [NotNull]
        [Mandatory]
        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public List<NotificationType>? NotificationTypes { get; set; }

        public List<CloudinaryImageModel>? Images { get; set; }

        public List<CloudinaryVideoModel>? Videos { get; set; }
    }
}