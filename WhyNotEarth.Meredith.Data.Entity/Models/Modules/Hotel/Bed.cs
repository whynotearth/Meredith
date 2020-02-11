namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Bed : IEntityTypeConfiguration<Bed>
    {
        public enum BedTypes
        {
            King,
            Queen,
            Twin,
            Single,
            LargeTwin,
            Double,

        };

        public BedTypes BedType { get; set; }

        public int Count { get; set; }

        public int Id { get; set; }

        public RoomType RoomType { get; set; }

        public int RoomTypeId { get; set; }

        public void Configure(EntityTypeBuilder<Bed> builder)
        {
            builder.ToTable("Beds", "ModuleHotel");
        }
    }
}