using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.BrowTricks
{
    public class FormItemEntityConfig : IEntityTypeConfiguration<FormItem>
    {
        public void Configure(EntityTypeBuilder<FormItem> builder)
        {
            builder.ToTable("FormItems", "ModuleBrowTricks");
            builder.Property(e => e.Options)
                .HasConversion(
                    o => string.Join(",", o ?? new List<string>()),
                    o => o.Split(",", StringSplitOptions.None).ToList());
        }
    }
}