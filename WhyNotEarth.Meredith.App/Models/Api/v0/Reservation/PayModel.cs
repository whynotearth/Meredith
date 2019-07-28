namespace WhyNotEarth.Meredith.App.Models.Api.v0.Reservation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PayModel
    {
        public decimal Amount { get; set; }

        public string Token { get; set; }
    }
}