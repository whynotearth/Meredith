using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    public class Amenity
    {
        public int Id { get; set; }

        public Hotel Hotel { get; set; }

        public int HotelId { get; set; }

        public ICollection<AmenityTranslation> Translations { get; set; }
    }

    public class AmenityEntityConfig : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.ToTable("Amenities", "ModuleHotel");
        }
    }
}