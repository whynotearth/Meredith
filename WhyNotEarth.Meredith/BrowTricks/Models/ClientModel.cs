using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Data.Entity.Models;
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

        public NotificationType NotificationType { get; set; }

        public string? Notes { get; set; }
    }
}