using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStart
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public List<string> DistributionGroups { get; set; } = null!;

        public JumpStartStatus Status { get; set; }

        public bool HasPdf { get; set; }
    }

    public enum JumpStartStatus : byte
    {
        Preview = 1,
        Sending = 2,
        Sent = 3
    }
}