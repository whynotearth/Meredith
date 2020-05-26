using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class Order : IEntityTypeConfiguration<Order>
    {
        public int Id { get; set; }

        public int PaymentMethodId { get; set; }

        public PaymentMethodType PaymentMethodType { get; set; }

        public List<OrderLine> OrderLines { get; set; }

        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "ModuleShop");
        }
    }
}