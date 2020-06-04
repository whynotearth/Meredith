using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using WhyNotEarth.Meredith.Validation;
using DayOfWeek = WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop.DayOfWeek;

namespace WhyNotEarth.Meredith.Models
{
    public class TenantModel
    {
        [NotNull]
        [Mandatory]
        public string? Name { get; set; }
        
        [NotNull]
        [Mandatory]
        public string? CompanySlug { get; set; }

        public string? Description { get; set; }

        [NotNull]
        [Mandatory]
        public PaymentMethodType? PaymentMethodType  { get; set; }

        [NotNull]
        [Mandatory]
        public NotificationType? NotificationType  { get; set; }

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

        public TimeSpan? OpeningTime { get; set; }

        public TimeSpan? ClosingTime { get; set; }
    }
}