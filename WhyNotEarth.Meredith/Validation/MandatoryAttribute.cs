using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.Validation
{
    public class MandatoryAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult(GetErrorMessage(validationContext.MemberName));
            }

            if (value is string stringValue)
            {
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    return new ValidationResult(GetErrorMessage(validationContext.MemberName));
                }
            }
            else if (value is ICollection collection)
            {
                if (collection.Count <= 0)
                {
                    return new ValidationResult(GetErrorMessage(validationContext.MemberName));
                }
            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage(string? fieldName)
        {
            if (fieldName is null)
            {
                return "Missing value for a required field.";
            }

            return $"The {fieldName} field is required.";
        }
    }
}