using System;
using System.Collections.Generic;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class VolkswagenSettingsModel
    {
        [Mandatory]
        public bool? EnableAutoSend { get; set; }

        public List<string>? DistributionGroups { get; set; }

        public TimeSpan? SendTime { get; set; }
    }
}