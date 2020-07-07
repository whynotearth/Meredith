using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using WhyNotEarth.Meredith.Validation;
using DayOfWeek = WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop.DayOfWeek;

namespace WhyNotEarth.Meredith.Tenant.Models
{
    public class TenantModel
    {
        [NotNull]
        [Mandatory]
        public string? Name { get; set; }
        
        public string? Description { get; set; }

        public List<string>? Tags { get; set; }

        public TimeSpan? DeliveryTime { get; set; }

        public decimal? DeliveryFee { get; set; }

        public string? LogoUrl { get; set; }

        [NotNull]
        [Mandatory]
        public List<PaymentMethodType>? PaymentMethodTypes  { get; set; }

        [NotNull]
        [Mandatory]
        public List<NotificationType>? NotificationTypes  { get; set; }

        [NotNull]
        [Mandatory]
        public List<BusinessHourModel>? BusinessHours { get; set; }
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