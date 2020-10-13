using Hangfire.Annotations;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class ConfirmTempPhoneNumberTokenModel
    {
        [NotNull]
        [Mandatory]
        public string? CompanySlug { get; set; }

        public string? TenantSlug { get; set; }
    }
}