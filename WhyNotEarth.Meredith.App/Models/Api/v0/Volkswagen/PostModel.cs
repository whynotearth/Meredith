using System;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class PostModel
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Headline { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        public decimal? Price { get; set; }

        public DateTime? EventDate { get; set; }

        public string? Image { get; set; }
    }
}