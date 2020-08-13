using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Volkswagen
{
    public class JumpStartEntityConfig : IEntityTypeConfiguration<JumpStart>
    {
        public void Configure(EntityTypeBuilder<JumpStart> builder)
        {
            builder.ToTable("JumpStarts", "ModuleVolkswagen");
            builder.Property(e => e.DistributionGroups)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(",", StringSplitOptions.None).ToList());
        }
    }
}