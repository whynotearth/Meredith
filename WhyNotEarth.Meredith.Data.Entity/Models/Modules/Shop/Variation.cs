using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class Variation : IEntityTypeConfiguration<Variation>
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int PriceId { get; set; }

        public Price Price { get; set; }

        public string Name { get; set; }

        public void Configure(EntityTypeBuilder<Variation> builder)
        {
            builder.ToTable("Variations", "ModuleShop");
            builder.Property(b => b.Name).IsRequired();
        }
    }
}