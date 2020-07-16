using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class Price
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }
    }

    public class PriceEntityConfig : IEntityTypeConfiguration<Price>
    {
        public void Configure(EntityTypeBuilder<Price> builder)
        {
            builder.ToTable("Prices", "ModuleShop");
            builder.Property(e => e.Amount).HasColumnType("numeric(10, 2)");
        }
    }
}