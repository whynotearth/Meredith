using System;
using System.ComponentModel.DataAnnotations;
using WhyNotEarth.Meredith.App.Validation;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class ArticleModel
    {
        [Mandatory]
        public string? CategorySlug { get; set; }

        [Mandatory]
        [MaxLength(80)]
        public string? Headline { get; set; }

        [Mandatory]
        [MaxLength(750)]
        public string? Description { get; set; }

        [Mandatory]
        public DateTime? Date { get; set; }

        public decimal? Price { get; set; }

        public DateTime? EventDate { get; set; }

        public string? Image { get; set; }
    }
}