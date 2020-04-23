using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class PaymentMethod : IEntityTypeConfiguration<PaymentMethod>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("PaymentMethods", "ModuleShop");
            builder.Property(b => b.Name).IsRequired();
        }
    }
}