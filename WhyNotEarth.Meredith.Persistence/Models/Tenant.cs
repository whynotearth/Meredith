using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Persistence.Models
{
    public class TenantEntityConfig : IEntityTypeConfiguration<Public.Tenant>
    {
        public void Configure(EntityTypeBuilder<Public.Tenant> builder)
        {
            builder.HasOne(tenant => tenant.Owner)
                .WithMany()
                .HasForeignKey(tenant => tenant.OwnerId);
            builder.Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(",", v ?? new List<string>()),
                    v => v.Split(",", StringSplitOptions.None).ToList());

            builder.HasIndex(u => u.PhoneNumber).IsUnique();
        }
    }
}