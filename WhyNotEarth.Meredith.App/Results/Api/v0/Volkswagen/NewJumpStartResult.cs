using System;
using System.Collections.Generic;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class NewJumpStartResult
    {
        public int Id { get; }

        public string Subject { get; }

        public DateTime DateTime { get; }

        public List<string> DistributionGroups { get; }

        public List<string>? Tags { get; set; }

        public string? Body { get; set; }

        public string? PdfUrl { get; set; }

        public NewJumpStartResult(NewJumpStart newJumpStart)
        {
            Id = newJumpStart.Id;
            Subject = newJumpStart.Subject;
            DateTime = newJumpStart.DateTime;
            DistributionGroups = newJumpStart.DistributionGroups;
            Tags = newJumpStart.Tags;
            Body = newJumpStart.Body;
            PdfUrl = newJumpStart.PdfUrl;
        }
    }
}