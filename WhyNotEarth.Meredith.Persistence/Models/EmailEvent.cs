using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Persistence.Models
{
    public class EmailEventEntityConfig : IEntityTypeConfiguration<EmailEvent>
    {
        public void Configure(EntityTypeBuilder<EmailEvent> builder)
        {
            builder.ToTable("EmailEvents", "public");
        }
    }
}