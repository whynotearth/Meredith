using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen
{
    public class Memo
    {
        public int Id { get; set; }

        public string Subject { get; set; } = null!;

        public string Date { get; set; } = null!;

        public string To { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? PdfUrl { get; set; }

        public List<string> DistributionGroups { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }

    public class MemoEntityConfig : IEntityTypeConfiguration<Memo>
    {
        public void Configure(EntityTypeBuilder<Memo> builder)
        {
            builder.ToTable("Memos", "ModuleVolkswagen");
            builder.Property(e => e.DistributionGroups)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(",", StringSplitOptions.None).ToList());
        }
    }
}