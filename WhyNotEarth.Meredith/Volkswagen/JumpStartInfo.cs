using System.Collections.Generic;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartInfo
    {
        public JumpStart JumpStart { get; }

        public List<string> Articles { get; set; }

        public ListStats ListStats { get; }

        public JumpStartInfo(JumpStart jumpStart, List<string> articles, ListStats listStats)
        {
            JumpStart = jumpStart;
            Articles = articles;
            ListStats = listStats;
        }
    }
}