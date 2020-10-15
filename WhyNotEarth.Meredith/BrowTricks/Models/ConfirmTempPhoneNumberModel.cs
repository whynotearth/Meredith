using Hangfire.Annotations;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class ConfirmTempPhoneNumberModel
    {
        [NotNull]
        [Mandatory]
        public string? Token { get; set; }
        
        [NotNull]
        [Mandatory]
        public string? TenantSlug { get; set; }
    }
}