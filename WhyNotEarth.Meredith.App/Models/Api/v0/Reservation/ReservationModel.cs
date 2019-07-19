namespace WhyNotEarth.Meredith.App.Models.Api.v0.Reservation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ReservationModel
    {
        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }
    }
}