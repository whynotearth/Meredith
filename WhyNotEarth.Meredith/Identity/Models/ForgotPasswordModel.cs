using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Identity.Models
{
    public class ForgotPasswordModel
    {
        [NotNull]
        [Mandatory]
        public string? CompanySlug { get; set; }

        [NotNull]
        [Mandatory]
        [EmailAddress]
        public string? Email { get; set; }

        [NotNull]
        [Mandatory]
        public string? ReturnUrl { get; set; }
    }
}