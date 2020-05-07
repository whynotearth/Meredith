using System;
using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class JumpStartResult
    {
        public int Id { get; }

        public DateTime DateTime { get; }

        public List<string> DistributionGroups { get; }

        public List<ArticleResult> Articles { get; }

        public JumpStartResult(JumpStart jumpStart)
        {
            Id = jumpStart.Id;
            DateTime = jumpStart.DateTime;
            DistributionGroups = jumpStart.DistributionGroups.Split(',').ToList();
            Articles = jumpStart.Articles.Select(item => new ArticleResult(item)).ToList() ?? new List<ArticleResult>();
        }
    }

    public class ArticleResult
    {
        public int Id { get; }

        public string? CategoryImage { get; }

        public string Headline { get; }

        public ArticleResult(Article article)
        {
            Id = article.Id;
            CategoryImage = article.Category.Image?.Url;
            Headline = article.Headline;
        }
    }
}