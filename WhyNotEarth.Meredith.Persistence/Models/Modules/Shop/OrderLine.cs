using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Shop
{
    public class OrderLineEntityConfig : IEntityTypeConfiguration<OrderLine>
    {
        public void Configure(EntityTypeBuilder<OrderLine> builder)
        {
            builder.ToTable("OrderLines", "ModuleShop");
        }
    }
}