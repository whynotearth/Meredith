using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Persistence.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class HotelReservationEntityConfig : ReservationEntityConfig
    {
        public void Configure(EntityTypeBuilder<HotelReservation> builder)
        {
            builder.Property(e => e.Start).HasColumnType("date");
            builder.Property(e => e.End).HasColumnType("date");
        }
    }
}