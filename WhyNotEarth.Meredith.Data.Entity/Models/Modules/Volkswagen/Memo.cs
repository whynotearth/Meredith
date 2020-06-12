using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen
{
    public class Memo : IEntityTypeConfiguration<Memo>
    {
        public int Id { get; set; }

        public string Subject { get; set; }
        
        public string Date { get; set; }
        
        public string To { get; set; }
        
        public string Description { get; set; }

        public List<string> DistributionGroups { get; set; }

        public DateTime CreationDateTime { get; set; }
        
        public void Configure(EntityTypeBuilder<Memo> builder)
        {
            builder.ToTable("Memos", "ModuleVolkswagen");
            builder.Property(b => b.Subject).IsRequired();
            builder.Property(b => b.Date).IsRequired();
            builder.Property(b => b.To).IsRequired();
            builder.Property(b => b.Description).IsRequired();
            builder.Property(b => b.DistributionGroups).IsRequired();
            builder.Property(e => e.DistributionGroups)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(",", StringSplitOptions.None).ToList());
        }
    }
}