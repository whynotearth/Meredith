using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Identity.Models
{
    public class ForgotPasswordResetModel
    {
        [NotNull]
        [Mandatory]
        public string? Email { get; set; }

        [NotNull]
        [Mandatory]
        public string? Token { get; set; }

        [NotNull]
        [Mandatory]
        public string? Password { get; set; }

        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }
    }
}