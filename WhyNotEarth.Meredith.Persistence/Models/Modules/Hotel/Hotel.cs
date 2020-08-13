using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class HotelEntityConfig : IEntityTypeConfiguration<Meredith.Hotel.Hotel>
    {
        public void Configure(EntityTypeBuilder<Meredith.Hotel.Hotel> builder)
        {
            builder.ToTable("Hotels", "ModuleHotel");
        }
    }
}