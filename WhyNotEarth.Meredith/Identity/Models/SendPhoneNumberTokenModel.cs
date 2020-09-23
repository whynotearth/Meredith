using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Identity.Models
{
    public class SendPhoneNumberTokenModel
    {
        [NotNull]
        [Mandatory]
        public string? CompanySlug { get; set; }

        public string? TenantSlug { get; set; }

        public string? PhoneNumber { get; set; }
    }
}