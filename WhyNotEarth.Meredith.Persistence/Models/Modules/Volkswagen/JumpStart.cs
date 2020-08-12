using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Volkswagen
{
    public class JumpStart
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public List<string> DistributionGroups { get; set; } = null!;

        public JumpStartStatus Status { get; set; }

        public bool HasPdf { get; set; }
    }

    public enum JumpStartStatus : byte
    {
        Preview = 1,
        Sending = 2,
        Sent = 3
    }

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