namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Hotel : IEntityTypeConfiguration<Hotel>
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

        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.ToTable("Hotels", "ModuleHotel");
        }
    }
}