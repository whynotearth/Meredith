using System;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class ArticleResult
    {
        public int Id { get; }

        public DateTime Date { get; }

        public ArticleCategoryResult Category { get; }

        public string Headline { get; }

        public ArticleResult(Article article)
        {
            Id = article.Id;
            Date = article.Date;
            Category = new ArticleCategoryResult(article.Category);
            Headline = article.Headline;
        }
    }
}