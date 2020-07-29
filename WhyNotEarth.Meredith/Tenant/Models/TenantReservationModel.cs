using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.Tenant.Models
{
    public class TenantReservationModel : IValidatableObject
    {
        [Required]
        public List<Order> Orders { get; set; } = null!;

        public decimal SubTotal { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal Amount { get; set; }

        public decimal Tax { get; set; }

        public DateTime DeliveryDateTime { get; set; }

        public int UserTimeZoneOffset { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string? Message { get; set; }

        public bool? WhatsappNotification { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DeliveryDateTime < DateTime.UtcNow)
            {
                yield return new ValidationResult("Invalid delivery date", new[] { nameof(DeliveryDateTime) });
            }
        }
    }

    public class Order
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public int Count { get; set; }

        [Required]
        public decimal Price { get; set; }

        public override string ToString()
        {
            return $"{Name}:{Count}({Price})";
        }
    }
}