using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Shop
{
    public class PaymentEntityConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments", "ModuleShop");
        }
    }
}