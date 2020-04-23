using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    public class HotelPrice : Price
    {
        public DateTime Date { get; set; }

        public int RoomTypeId { get; set; }

        public RoomType RoomType { get; set; }

        public void Configure(EntityTypeBuilder<HotelPrice> builder)
        {
            builder.Property(e => e.Date).HasColumnType("date");
        }
    }
}