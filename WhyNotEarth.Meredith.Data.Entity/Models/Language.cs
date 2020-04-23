using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Language : IEntityTypeConfiguration<Language>
    {
        public int Id { get; set; }

        public string Culture { get; set; }

        public string Name { get; set; }

        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Languages", "public");
        }
    }
}
