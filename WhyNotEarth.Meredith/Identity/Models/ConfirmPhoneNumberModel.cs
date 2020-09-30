using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Identity.Models
{
    public class ConfirmPhoneNumberModel
    {
        [NotNull]
        [Mandatory]
        public string? Token { get; set; }
    }
}