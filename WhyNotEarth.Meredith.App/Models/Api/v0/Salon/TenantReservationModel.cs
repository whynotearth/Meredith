using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Salon
{
    public class TenantReservationModel
    {
        [Required]
        public List<Order> Orders { get; set; } = null!;

        public decimal SubTotal { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal Amount { get; set; }

        public decimal Tax { get; set; }

        public DateTime DeliveryDateTime { get; set; }

        public int UserTimeZoneOffset { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string? Message { get; set; }
    }

    public class Order
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public int Count { get; set; }

        [Required]
        public decimal Price { get; set; }

        public override string ToString()
        {
            return $"{Name}:{Count}({Price})";
        }
    }
}