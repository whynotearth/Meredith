using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.Identity.Models
{
    public class RegisterModel : IValidatableObject
    {
        public string? Email { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? GoogleLocation { get; set; }

        public string? TenantSlug { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Email is null && UserName is null)
            {
                yield return new ValidationResult("Provide email or username",
                    new[] { nameof(Email), nameof(UserName) });
            }

            if (Email is null && PhoneNumber is null)
            {
                yield return new ValidationResult("Provide email or phone number",
                    new[] { nameof(Email), nameof(PhoneNumber) });
            }
        }
    }
}