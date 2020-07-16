using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    public class Space
    {
        public int Id { get; set; }

        public int HotelId { get; set; }

        public Hotel Hotel { get; set; } = null!;

        public ICollection<SpaceTranslation>? Translations { get; set; }
    }

    public class SpaceEntityConfig : IEntityTypeConfiguration<Space>
    {
        public void Configure(EntityTypeBuilder<Space> builder)
        {
            builder.ToTable("Spaces", "ModuleHotel");
        }
    }
}