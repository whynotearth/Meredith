using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Hotel;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class RoomEntityConfig : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.Property(e => e.Number).HasMaxLength(16);
            builder.ToTable("Rooms", "ModuleHotel");
        }
    }
}