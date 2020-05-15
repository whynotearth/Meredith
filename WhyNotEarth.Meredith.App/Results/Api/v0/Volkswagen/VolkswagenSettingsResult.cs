using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class VolkswagenSettingsResult
    {
        public List<string> DistributionGroups { get; }

        public bool EnableAutoSend { get; }

        public TimeSpan? SendTime { get; }

        public VolkswagenSettingsResult(List<string> distributionGroups, bool enableAutoSend, TimeSpan? sendTime)
        {
            DistributionGroups = distributionGroups;
            EnableAutoSend = enableAutoSend;
            SendTime = sendTime;
        }
    }
}