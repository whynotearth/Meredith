using System;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class ArticleModel
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(80)]
        public string Headline { get; set; } = null!;

        [Required]
        [MaxLength(750)]
        public string Description { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        public decimal? Price { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EventDate { get; set; }

        public string? Image { get; set; }
    }
}