using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Persistence.Models
{
    public class EmailEntityConfig : IEntityTypeConfiguration<Public.Email>
    {
        public void Configure(EntityTypeBuilder<Public.Email> builder)
        {
            builder.ToTable("Emails", "public");
        }
    }
}