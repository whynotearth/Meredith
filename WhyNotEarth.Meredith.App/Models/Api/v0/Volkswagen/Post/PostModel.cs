using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen.Post
{
    public class PostModel
    {
        public int CategoryId { get; set; }

        [MaxLength(50)]
        public string Headline  { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        public List<string> Images { get; set; }
    }
}