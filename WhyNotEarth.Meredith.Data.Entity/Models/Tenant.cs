#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.Data.Entity.Models
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
    }

    public class TenantImage : Image
    {
    }

    public class TenantEntityConfig : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.HasOne(tenant => tenant.Owner)
                .WithMany()
                .HasForeignKey(tenant => tenant.OwnerId);
            builder.Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(",", v ?? new List<string>()),
                    v => v.Split(",", StringSplitOptions.None).ToList());
        }
    }
}