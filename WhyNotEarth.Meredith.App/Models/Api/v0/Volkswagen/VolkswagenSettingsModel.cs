using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WhyNotEarth.Meredith.App.Validation;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class VolkswagenSettingsModel : IValidatableObject
    {
        [Mandatory]
        public bool? EnableAutoSend { get; set; }

        public List<string>? DistributionGroups { get; set; }

        public TimeSpan? SendTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EnableAutoSend.HasValue && EnableAutoSend.Value)
            {
                if (DistributionGroups is null || !DistributionGroups.Any())
                {
                    yield return new ValidationResult(
                        $"With current value of {nameof(EnableAutoSend)} the {nameof(DistributionGroups)} field is required.",
                        new[] {nameof(DistributionGroups)});
                }

                if (SendTime is null)
                {
                    yield return new ValidationResult(
                        $"With current value of {nameof(EnableAutoSend)} the {nameof(SendTime)} field is required.",
                        new[] {nameof(SendTime)});
                }
            }
        }
    }
}