using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Hotel;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class SpaceEntityConfig : IEntityTypeConfiguration<Space>
    {
        public void Configure(EntityTypeBuilder<Space> builder)
        {
            builder.ToTable("Spaces", "ModuleHotel");
        }
    }
}