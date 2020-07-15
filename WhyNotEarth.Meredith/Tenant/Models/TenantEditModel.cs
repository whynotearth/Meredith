using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.Tenant.Models
{
    public class TenantEditModel : IValidatableObject
    {
        public bool? HasPromotion { get; set; }

        public int? PromotionPercent { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!HasPromotion.HasValue && PromotionPercent.HasValue)
            {
                yield return new ValidationResult("Missing promotion status", new[] {nameof(HasPromotion)});
            }
            else if (HasPromotion.HasValue)
            {
                if (!PromotionPercent.HasValue)
                {
                    yield return new ValidationResult("Missing discount value", new[] {nameof(PromotionPercent)});
                }
                else if (PromotionPercent < 0 || PromotionPercent > 100)
                {
                    yield return new ValidationResult("Invalid discount value", new[] {nameof(PromotionPercent)});
                }
            }
        }
    }
}