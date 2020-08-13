using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Hotel;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class RoomTypeEntityConfig : IEntityTypeConfiguration<RoomType>
    {
        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            builder.Property(e => e.Name);
            builder.ToTable("RoomTypes", "ModuleHotel");
        }
    }
}