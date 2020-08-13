using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Persistence.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Hotel
{
    public class HotelPriceEntityConfig : PriceEntityConfig
    {
        public void Configure(EntityTypeBuilder<HotelPrice> builder)
        {
            builder.Property(e => e.Date).HasColumnType("date");
        }
    }
}