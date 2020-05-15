using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class VolkswagenSettingsModel
    {
        [Required]
        public List<string> DistributionGroups { get; set; } = null!;

        [Required]
        public bool? EnableAutoSend { get; set; } = null!;

        public TimeSpan? SendTime { get; set; }
    }
}