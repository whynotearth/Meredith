using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Identity.Models
{
    public class ConfirmPhoneNumberTokenModel
    {
        [NotNull]
        [Mandatory]
        public string? CompanySlug { get; set; }

        public string? TenantSlug { get; set; }

        public string? PhoneNumber { get; set; }
    }
}