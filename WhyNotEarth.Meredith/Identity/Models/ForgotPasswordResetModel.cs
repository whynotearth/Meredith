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

        public int? UserId { get; set; }

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
            var setFields = (Email == null ? 0 : 1) + (UserName == null ? 0 : 1) + (!UserId.HasValue ? 0 : 1);
            if (setFields == 0)
            {
                yield return new ValidationResult("Provide email, username or user ID",
                    new[] { nameof(Email), nameof(UserName), nameof(UserId) });
            }
            else if (setFields > 1)
            {
                yield return new ValidationResult("Provide email, username or user ID",
                    new[] { nameof(Email), nameof(UserName), nameof(UserId) });
            }
        }
    }
}