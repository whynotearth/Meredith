using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class ArticleModel
    {
        [NotNull]
        [Mandatory]
        public string? CategorySlug { get; set; }

        [NotNull]
        [Mandatory]
        [MaxLength(80)]
        public string? Headline { get; set; }

        [NotNull]
        [Mandatory]
        [MaxLength(750)]
        public string? Description { get; set; }

        [NotNull]
        [Mandatory]
        public DateTime? Date { get; set; }

        public DateTime? EventDate { get; set; }

        public ImageModel? Image { get; set; }

        public string? Excerpt { get; set; }
    }
}