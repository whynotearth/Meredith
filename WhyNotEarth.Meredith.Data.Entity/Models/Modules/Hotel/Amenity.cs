namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Amenity : IEntityTypeConfiguration<Amenity>
    {
        public int Id { get; set; }

        public Hotel Hotel { get; set; }

        public int HotelId { get; set; }

        public string Text { get; set; }

        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.ToTable("Amenities", "ModuleHotel");
        }
    }
}