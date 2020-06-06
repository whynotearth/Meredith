using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class ProductAttribute : IEntityTypeConfiguration<ProductAttribute>
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int PriceId { get; set; }

        public Price Price { get; set; }

        public string Name { get; set; }

        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.ToTable("ProductAttributes", "ModuleShop");
            builder.Property(b => b.Name).IsRequired();
        }
    }
}
