using System;
using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class ArticleDailyResult
    {
        public int? JumpStartId { get; set; }

        public DateTime Date { get; }

        public List<ArticleResult> Articles { get; }

        public ArticleDailyResult(DateTime date, List<Article> articles)
        {
            JumpStartId = articles.FirstOrDefault(item => item.JumpStartId != null)?.JumpStartId;
            Date = date;
            Articles = articles.Select(item => new ArticleResult(item)).ToList();
        }
    }
}