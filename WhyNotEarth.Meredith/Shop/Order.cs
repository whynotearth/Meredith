using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Shop
{
    public class Order
    {
        public int Id { get; set; }

        public PaymentMethodType PaymentMethodType { get; set; }

        public List<OrderLine> OrderLines { get; set; } = null!;
    }
}