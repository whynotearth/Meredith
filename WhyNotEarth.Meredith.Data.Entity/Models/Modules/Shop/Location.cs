using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class Location
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }

    public class LocationEntityConfig : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations", "ModuleShop");
        }
    }
}