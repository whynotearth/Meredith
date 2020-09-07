using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class FormTemplateModel
    {
        [NotNull]
        [Mandatory]
        public string? Name { get; set; }

        [NotNull]
        [Mandatory]
        public FormTemplateType? Type { get; set; }

        [NotNull]
        [Mandatory]
        public List<FormItemModel>? Items { get; set; }
    }

    public class FormItemModel : IValidatableObject
    {
        [NotNull]
        [Mandatory]
        public FormItemType? Type { get; set; }

        [NotNull]
        [Mandatory]
        public bool? IsRequired { get; set; }

        [NotNull]
        [Mandatory]
        public string Value { get; set; } = null!;

        public List<string>? Options { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == FormItemType.Checklist || Type == FormItemType.MultipleChoice)
            {
                if (Options is null)
                {
                    yield return new ValidationResult("Missing options", new[] { nameof(Options) });
                }
            }
            else
            {
                if (Options != null)
                {
                    yield return new ValidationResult("Invalid options value", new[] { nameof(Options) });
                }
            }
        }
    }
}