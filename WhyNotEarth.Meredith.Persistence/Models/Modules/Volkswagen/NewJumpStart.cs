using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Volkswagen
{
    public class NewJumpStartEntityConfig : IEntityTypeConfiguration<NewJumpStart>
    {
        public void Configure(EntityTypeBuilder<NewJumpStart> builder)
        {
            builder.ToTable("NewJumpStarts", "ModuleVolkswagen");
            builder.Property(e => e.DistributionGroups)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(",", StringSplitOptions.None).ToList());
            builder.Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(",", v ?? new List<string>()),
                    v => v.Split(",", StringSplitOptions.None).ToList());
        }
    }
}