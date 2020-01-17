namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Space : IEntityTypeConfiguration<Space>
    {
        public Hotel Hotel { get; set; }

        public int HotelId { get; set; }

        public int Id { get; set; }

        public ICollection<SpaceTranslation> Translations { get; set; }

        public void Configure(EntityTypeBuilder<Space> builder)
        {
            builder.ToTable("Spaces", "ModuleHotel");
        }
    }
}