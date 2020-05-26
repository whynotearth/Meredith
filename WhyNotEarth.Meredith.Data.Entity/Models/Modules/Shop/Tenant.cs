using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class Tenant : IEntityTypeConfiguration<Tenant>
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public string Slug { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string Name { get; set; }

        public TenantImage Logo { get; set; }

        public string Tags { get; set; }

        public TimeSpan DeliveryTime { get; set; }

        public decimal DeliveryFee { get; set; }

        public ICollection<BusinessHour> BusinessHours { get; set; }

        public PaymentMethodType PaymentMethodType { get; set; }

        public NotificationType NotificationType { get; set; }

        public string Description { get; set; }

        public ICollection<Page> Pages { get; set; }

        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
        }
    }

    public class TenantImage : Image
    {
    }
}