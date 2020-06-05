using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Tenant : IEntityTypeConfiguration<Tenant>
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public string Slug { get; set; }

        public int OwnerId { get; set; }

        public User Owner { get; set; }

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
            builder.HasOne(b => b.Owner)
                .WithOne(i => i.Tenant)
                .HasForeignKey<Tenant>(b => b.OwnerId);
        }
    }

    public class TenantImage : Image
    {
    }
}