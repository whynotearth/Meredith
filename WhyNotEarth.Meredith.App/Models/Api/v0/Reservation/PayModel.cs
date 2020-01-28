using System.Collections.Generic;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Reservation
{
    public class PayModel
    {
        public decimal Amount { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}