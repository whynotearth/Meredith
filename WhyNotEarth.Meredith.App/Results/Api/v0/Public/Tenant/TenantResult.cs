using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Shop;
using DayOfWeek = WhyNotEarth.Meredith.Shop.DayOfWeek;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Tenant
{
    public class TenantResult
    {
        public string Slug { get; }

        public string Name { get; }

        public string? LogoUrl { get; }

        public List<string>? Tags { get; }

        public TimeSpan DeliveryTime { get; }

        public decimal DeliveryFee { get; }

        public string? Description { get; }

        public List<PaymentMethodType> PaymentMethodTypes { get; }

        public List<NotificationType> NotificationTypes { get; }

        public List<BusinessHourResult>? BusinessHours { get; }

        public bool IsActive { get; }

        public AddressResult Address { get; }

        public string? PhoneNumber { get; }

        public string? FacebookUrl { get; }

        public string? WhatsAppNumber { get; }

        public bool HasPromotion { get; }

        public int PromotionPercent { get; }

        public object? SeoSchema { get; }

        public TenantResult(Meredith.Public.Tenant tenant)
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
            BusinessHours = tenant.BusinessHours?.Select(item => new BusinessHourResult(item))?.ToList();
            IsActive = tenant.IsActive;
            Address = new AddressResult(tenant.Address);
            PhoneNumber = tenant.PhoneNumber;
            FacebookUrl = tenant.FacebookUrl;
            WhatsAppNumber = tenant.WhatsAppNumber;
            HasPromotion = tenant.HasPromotion;
            PromotionPercent = tenant.PromotionPercent;
        }

        public TenantResult(Meredith.Public.Tenant tenant, string schema) : this(tenant)
        {
            SeoSchema = schema is null ? null : JsonConvert.DeserializeObject<dynamic>(schema);
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