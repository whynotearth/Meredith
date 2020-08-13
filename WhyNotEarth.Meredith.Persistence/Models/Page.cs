using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Persistence.Models
{
    public class PageEntityConfig : IEntityTypeConfiguration<Page>
    {
        public void Configure(EntityTypeBuilder<Page> builder)
        {
            builder.Property(e => e.Custom).HasColumnType("jsonb");
        }
    }
}