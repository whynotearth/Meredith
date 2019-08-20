namespace WhyNotEarth.Meredith.App.Models.Api.v0.Price
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PriceModel
    {
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int RoomTypeId { get; set; }
    }
}
