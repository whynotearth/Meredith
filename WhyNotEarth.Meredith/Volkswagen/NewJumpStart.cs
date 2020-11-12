using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class NewJumpStart
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public string Subject { get; set; } = null!;

        public List<string> DistributionGroups { get; set; } = null!;

        public List<string>? Tags { get; set; }

        public string? Body { get; set; }

        public string? PdfUrl { get; set; }

        public NewJumpStartStatus Status { get; set; }
    }

    public enum NewJumpStartStatus : byte
    {
        Preview = 1,
        Sending = 2,
        Sent = 3
    }
}