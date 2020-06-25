using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Tenant
{
    public class TenantResult
    {
        public string Slug { get; }

        public string Name { get; }

        public string? Logo { get; }

        public List<string> Tags { get; }

        public TimeSpan DeliveryTime { get; set; }

        public decimal DeliveryFee { get; set; }

        public TenantResult(Data.Entity.Models.Tenant tenant)
        {
            Slug = tenant.Slug;
            Name = tenant.Name;
            Logo = tenant.Logo?.Url;
            Tags = tenant.Tags;
            DeliveryTime = tenant.DeliveryTime;
            DeliveryFee = tenant.DeliveryFee;
        }
    }
}