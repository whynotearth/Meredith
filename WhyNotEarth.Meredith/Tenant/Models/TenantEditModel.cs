using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.Tenant.Models
{
    public class TenantEditModel : IValidatableObject
    {
        public string? Name { get; set; }

        public List<string>? Tags { get; set; }

        public string? Description { get; set; }

        public string? InstagramUrl { get; set; }

        public string? FacebookUrl { get; set; }

        public string? YouTubeUrl { get; set; }

        public string? WhatsAppNumber { get; set; }

        public bool? HasPromotion { get; set; }

        public int? PromotionPercent { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!HasPromotion.HasValue && PromotionPercent.HasValue)
            {
                yield return new ValidationResult("Missing promotion status", new[] { nameof(HasPromotion) });
            }
            else if (HasPromotion.HasValue)
            {
                if (!PromotionPercent.HasValue)
                {
                    yield return new ValidationResult("Missing discount value", new[] { nameof(PromotionPercent) });
                }
                else if (PromotionPercent < 0 || PromotionPercent > 100)
                {
                    yield return new ValidationResult("Invalid discount value", new[] { nameof(PromotionPercent) });
                }
            }
        }
    }
}