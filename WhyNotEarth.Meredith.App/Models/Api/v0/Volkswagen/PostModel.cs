using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class PostModel
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public string Headline  { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        [Required]
        public DateTime Date { get; set; }
        
        public decimal? Price { get; set; }
        
        public DateTime? EventDate { get; set; }

        public List<string>? Images { get; set; }
    }
}