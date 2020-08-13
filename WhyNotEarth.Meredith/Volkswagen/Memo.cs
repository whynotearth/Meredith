using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class Memo
    {
        public int Id { get; set; }

        public string Subject { get; set; } = null!;

        public string Date { get; set; } = null!;

        public string To { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? PdfUrl { get; set; }

        public List<string> DistributionGroups { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}