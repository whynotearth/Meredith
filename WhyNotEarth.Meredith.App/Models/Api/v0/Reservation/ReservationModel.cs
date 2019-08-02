namespace WhyNotEarth.Meredith.App.Models.Api.v0.Reservation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ReservationModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public DateTime End { get; set; }

        public string Message { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        public int NumberOfGuests { get; set; }

        public PayModel Payment { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public DateTime Start { get; set; }
    }
}