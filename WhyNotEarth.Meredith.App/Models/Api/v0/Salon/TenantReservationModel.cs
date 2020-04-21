using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Salon
{
    public class TenantReservationModel
    {
        [Required]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public List<Order> Orders { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public decimal SubTotal { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal Amount { get; set; }

        public DateTime DeliveryDateTime { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public string UserTimeZone { get; set; }

        public string PaymentMethod { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public string? Message { get; set; }
    }

    public class Order
    {
        [Required]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

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