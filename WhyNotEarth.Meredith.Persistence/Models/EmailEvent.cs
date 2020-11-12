using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Emails;

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