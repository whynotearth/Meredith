using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Identity.Models
{
    public class ConfirmEmailTokenModel
    {
        [NotNull]
        [Mandatory]
        public string? CompanySlug { get; set; }

        [NotNull]
        [Mandatory]
        public string? ReturnUrl { get; set; }
    }
}