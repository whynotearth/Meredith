#nullable enable

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class Order
    {
        public int Id { get; set; }

        public PaymentMethodType PaymentMethodType { get; set; }

        public List<OrderLine> OrderLines { get; set; } = null!;
    }

    public class OrderEntityConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "ModuleShop");
        }
    }
}