using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen
{
    public class NewJumpStart
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public string Subject { get; set; } = null!;

        public List<string> DistributionGroups { get; set; } = null!;

        public List<string>? Tags { get; set; }

        public string? Body { get; set; }

        public string? PdfUrl { get; set; }

        public NewJumpStartStatus Status { get; set; }
    }

    public enum NewJumpStartStatus : byte
    {
        Preview = 1,
        Sending = 2,
        Sent = 3
    }

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