namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Amenity : IEntityTypeConfiguration<Amenity>
    {
        public int Id { get; set; }

        public RoomType RoomType { get; set; }

        public int RoomTypeId { get; set; }

        public string Text { get; set; }

        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.ToTable("Amenities", "ModuleHotel");
        }
    }
}