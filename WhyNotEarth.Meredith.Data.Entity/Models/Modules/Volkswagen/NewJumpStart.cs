using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen
{
    public class NewJumpStart : IEntityTypeConfiguration<NewJumpStart>
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public string Subject { get; set; }

        public List<string> DistributionGroups { get; set; }

        public List<string> Tags { get; set; }

        public string Body { get; set; }

        public NewJumpStartStatus Status { get; set; }

        public void Configure(EntityTypeBuilder<NewJumpStart> builder)
        {
            builder.ToTable("NewJumpStarts", "ModuleVolkswagen");
            builder.Property(b => b.Subject).IsRequired();
            builder.Property(b => b.DistributionGroups).IsRequired();
            builder.Property(b => b.Body).IsRequired();
            builder.Property(e => e.DistributionGroups)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(",", StringSplitOptions.None).ToList());
            builder.Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(",", StringSplitOptions.None).ToList());
        }
    }

    public enum NewJumpStartStatus : byte
    {
        Preview = 1,
        Sending = 2,
        Sent = 3
    }
}