using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class VolkswagenSettingsResult
    {
        public List<string> DistributionGroups { get; }

        public TimeSpan SendTime { get; }

        public VolkswagenSettingsResult(List<string> distributionGroups, TimeSpan sendTime)
        {
            DistributionGroups = distributionGroups;
            SendTime = sendTime;
        }
    }
}