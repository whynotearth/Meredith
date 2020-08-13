using System;
using System.Collections.Generic;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.Public
{
    public class Tenant
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public int OwnerId { get; set; }

        public User Owner { get; set; } = null!;

        public string Name { get; set; } = null!;

        public TenantImage? Logo { get; set; }

        public List<string>? Tags { get; set; }

        public TimeSpan DeliveryTime { get; set; }

        public decimal DeliveryFee { get; set; }

        public ICollection<BusinessHour> BusinessHours { get; set; } = null!;

        public PaymentMethodType PaymentMethodType { get; set; }

        public NotificationType NotificationType { get; set; }

        public string? Description { get; set; }

        public ICollection<Page>? Pages { get; set; }

        public int? AddressId { get; set; }

        public Address? Address { get; set; }

        public bool IsActive { get; set; }

        public string? PhoneNumber { get; set; }

        public string? FacebookUrl { get; set; }

        public string? WhatsAppNumber { get; set; }

        public bool HasPromotion { get; set; }

        public int PromotionPercent { get; set; }
    }

    public class TenantImage : Image
    {
    }
}