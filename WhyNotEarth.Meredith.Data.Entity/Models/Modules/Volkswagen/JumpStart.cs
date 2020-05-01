using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen
{
    public class JumpStart : IEntityTypeConfiguration<JumpStart>
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public string DistributionGroups { get; set; }

        public JumpStartStatus Status { get; set; }

        public bool HasPdf { get; set; }

        public List<Post> Posts { get; set; }

        public void Configure(EntityTypeBuilder<JumpStart> builder)
        {
            builder.ToTable("JumpStarts", "ModuleVolkswagen");
            builder.Property(b => b.DistributionGroups).IsRequired();
        }
    }

    public enum JumpStartStatus
    {
        ReadyToSend = 1,
        Sent = 2
    }
}