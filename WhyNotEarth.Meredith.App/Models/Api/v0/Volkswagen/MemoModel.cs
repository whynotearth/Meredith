using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class MemoModel
    {
        [Required]
        public List<string> DistributionGroups { get; set; } = null!;

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        public string Date { get; set; } = null!;

        [Required]
        public string To { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;
    }
}