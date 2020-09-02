namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Platform
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using WhyNotEarth.Meredith.Public;
    public class PlatformConfig : IEntityTypeConfiguration<Platform>
    {
        public void Configure(EntityTypeBuilder<Platform> builder)
        {
            builder.ToTable("Platforms", "Platform");
        }
    }
}