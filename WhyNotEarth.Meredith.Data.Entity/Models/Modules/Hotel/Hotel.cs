using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    public class Hotel
    {
        public ICollection<Amenity> Amenities { get; set; }

        public Company Company { get; set; }

        public int? CompanyId { get; set; }

        public int Id { get; set; }

        public Page Page { get; set; }

        public int PageId { get; set; }

        public ICollection<RoomType> RoomTypes { get; set; }

        public ICollection<Rule> Rules { get; set; }

        public ICollection<Space> Spaces { get; set; }

        public ICollection<HotelTranslation> Translations { get; set; }
    }

    public class HotelEntityConfig : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.ToTable("Hotels", "ModuleHotel");
        }
    }
}