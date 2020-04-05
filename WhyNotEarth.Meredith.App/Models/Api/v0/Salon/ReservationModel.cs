using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Salon
{
    public class ReservationModel
    {
        public List<Order> Orders { get; set; }

        public decimal SubTotal { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal Amount { get; set; }

        public DateTime DeliveryDateTime { get; set; }

        public string Message { get; set; }
    }

    public class Order
    {
        public string Name { get; set; }

        public int Count { get; set; }

        public decimal Price { get; set; }

        public override string ToString()
        {
            return $"{Name}:{Count}({Price})";
        }
    }
}