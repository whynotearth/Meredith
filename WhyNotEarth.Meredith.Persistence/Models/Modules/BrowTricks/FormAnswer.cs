using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.BrowTricks
{
    public class FormAnswerEntityConfig : IEntityTypeConfiguration<FormAnswer>
    {
        public void Configure(EntityTypeBuilder<FormAnswer> builder)
        {
            builder.ToTable("FormAnswers", "ModuleBrowTricks");
            builder.Property(e => e.Options)
                .HasConversion(
                    o => string.Join(",", o ?? new List<string>()),
                    o => o.Split(",", StringSplitOptions.None).ToList());
            builder.Property(e => e.Answers)
                .HasConversion(
                    o => string.Join(",", o ?? new List<string>()),
                    o => o.Split(",", StringSplitOptions.None).ToList());
        }
    }
}