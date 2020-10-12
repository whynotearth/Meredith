using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Shop;
using WhyNotEarth.Meredith.Validation;
using DayOfWeek = WhyNotEarth.Meredith.Shop.DayOfWeek;

namespace WhyNotEarth.Meredith.Tenant.Models
{
    public class TenantCreateModel
    {
        [NotNull]
        [Mandatory]
        public string? Name { get; set; }
        
        public TenantImageModel? Logo { get; set; }

        public string? Description { get; set; }

        public List<string>? Tags { get; set; }

        public TimeSpan? DeliveryTime { get; set; }

        public decimal? DeliveryFee { get; set; }

        public string? LogoUrl { get; set; }

        public List<PaymentMethodType>? PaymentMethodTypes { get; set; }

        public List<NotificationType>? NotificationTypes { get; set; }

        public List<BusinessHourModel> BusinessHours { get; set; } = new List<BusinessHourModel>();

        public string? PhoneNumber { get; set; }

        public string? FacebookUrl { get; set; }

        public string? WhatsAppNumber { get; set; }
    }

    public class BusinessHourModel
    {
        [NotNull]
        [Mandatory]
        public DayOfWeek? DayOfWeek { get; set; }

        [NotNull]
        [Mandatory]
        public bool? IsClosed { get; set; }

        [DataType(DataType.Time)]
        public DateTime? OpeningTime { get; set; }

        [DataType(DataType.Time)]
        public DateTime? ClosingTime { get; set; }
    }
}