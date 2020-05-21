using System;
using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class JumpStartStatResult
    {
        public int Id { get; }

        public List<string> DistributionGroups { get; }

        public DateTime DateTime { get; }

        public List<string> Articles { get; }

        public int OpenPercentage { get; }
        
        public JumpStartStatResult(JumpStartInfo jumpStartInfo)
        {
            Id = jumpStartInfo.JumpStart.Id;
            DistributionGroups = jumpStartInfo.JumpStart.DistributionGroups.Split(',').ToList();
            DateTime = jumpStartInfo.JumpStart.DateTime;
            Articles = jumpStartInfo.Articles;
            OpenPercentage = jumpStartInfo.ListStats.OpenPercentage;
        }
    }
}