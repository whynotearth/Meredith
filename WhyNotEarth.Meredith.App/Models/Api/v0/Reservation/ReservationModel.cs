using System;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Reservation
{
    public class ReservationModel
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public DateTime End { get; set; }

        public string? Message { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public int NumberOfGuests { get; set; }

        // TODO: Add required to this field
        public string? PhoneCountry { get; set; }

        [Required]
        public string Phone { get; set; } = null!;

        [Required]
        public DateTime Start { get; set; }
    }
}