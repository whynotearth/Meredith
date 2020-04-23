using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class Location : IEntityTypeConfiguration<Location>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations", "ModuleShop");
            builder.Property(b => b.Name).IsRequired();
        }
    }
}