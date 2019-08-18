using System;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Price
{
    public class PriceModel
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Amount { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int RoomTypeId { get; set; }
    }
}
