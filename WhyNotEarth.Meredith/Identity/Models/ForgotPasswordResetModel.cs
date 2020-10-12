using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Identity.Models
{
    public class ForgotPasswordResetModel : IValidatableObject
    {
        [EmailAddress]
        public string? Email { get; set; }

        public string? UserName { get; set; }

        [NotNull]
        [Mandatory]
        public string? Token { get; set; }

        [NotNull]
        [Mandatory]
        public string? Password { get; set; }

        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Email is null && UserName is null)
            {
                yield return new ValidationResult("Provide email or username",
                    new[] { nameof(Email), nameof(UserName) });
            }
            else if (Email != null && UserName != null)
            {
                yield return new ValidationResult("Provide email or username",
                    new[] { nameof(Email), nameof(UserName) });
            }
        }
    }
}