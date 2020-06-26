using System;
using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using DayOfWeek = WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop.DayOfWeek;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Tenant
{
    public class TenantResult
    {
        public string Slug { get; }

        public string Name { get; }

        public string? LogoUrl { get; }

        public List<string> Tags { get; }

        public TimeSpan DeliveryTime { get; }

        public decimal DeliveryFee { get; }

        public string Description { get; }
        
        public List<PaymentMethodType> PaymentMethodTypes { get; }

        public List<NotificationType> NotificationTypes { get; }

        public List<BusinessHourResult> BusinessHours { get; }
        
        public TenantResult(Data.Entity.Models.Tenant tenant)
        {
            Slug = tenant.Slug;
            Name = tenant.Name;
            LogoUrl = tenant.Logo?.Url;
            Tags = tenant.Tags;
            DeliveryTime = tenant.DeliveryTime;
            DeliveryFee = tenant.DeliveryFee;
            Description = tenant.Description;
            PaymentMethodTypes = tenant.PaymentMethodType.ToList();
            NotificationTypes = tenant.NotificationType.ToList();
            BusinessHours = tenant.BusinessHours.Select(item => new BusinessHourResult(item)).ToList();
        }
    }

    public class BusinessHourResult
    {
        public DayOfWeek DayOfWeek { get; }

        public bool IsClosed { get; set; }

        public TimeSpan? OpeningTime { get; set; }

        public TimeSpan? ClosingTime { get; set; }

        public BusinessHourResult(BusinessHour businessHour)
        {
            DayOfWeek = businessHour.DayOfWeek;
            IsClosed = businessHour.IsClosed;
            OpeningTime = businessHour.OpeningTime;
            ClosingTime = businessHour.ClosingTime;
        }
    }
}