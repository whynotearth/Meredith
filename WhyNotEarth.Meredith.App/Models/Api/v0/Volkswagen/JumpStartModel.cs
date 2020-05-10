using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class JumpStartModel
    {
        public int? Id { get; set; }

        [Required]
        public DateTime? DateTime { get; set; }

        [Required]
        public List<string> DistributionGroups { get; set; } = null!;

        [Required]
        public List<int> ArticleIds { get; set; } = null!;
    }
}