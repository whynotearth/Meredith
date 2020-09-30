using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Identity.Models
{
    public class LoginModel : IValidatableObject
    {
        public string? UserName { get; set; }

        public string? Email { get; set; }

        [NotNull]
        [Mandatory]
        public string? Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Email is null && UserName is null)
            {
                yield return new ValidationResult("Provide email or username",
                    new[] { nameof(Email), nameof(UserName) });
            }
        }
    }
}