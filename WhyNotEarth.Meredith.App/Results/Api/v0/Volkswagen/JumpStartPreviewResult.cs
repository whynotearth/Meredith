using System;
using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class JumpStartPreviewResult
    {
        public DateTime Date { get; }

        public List<ArticleResult> Article { get; }

        public JumpStartPreviewResult(DateTime date, List<Article>? articles)
        {
            Date = date;
            Article = articles.Select(item => new ArticleResult(item)).ToList() ?? new List<ArticleResult>();
        }
    }

    public class ArticleResult
    {
        public int Id { get; }

        public ArticleCategoryResult Category { get; }

        public string Headline { get; }

        public string Description { get; }

        public DateTime Date { get; }

        public decimal? Price { get; }

        public DateTime? EventDate { get; }

        public ImageResult? Image { get; }

        public ArticleResult(Article article)
        {
            Id = article.Id;
            Category = new ArticleCategoryResult(article.Category);
            Headline = article.Headline;
            Description = article.Description;
            Date = article.Date;
            Price = article.Price;
            EventDate = article.EventDate;

            if (article.Image != null)
            {
                Image = new ImageResult(article.Image);
            }
        }
    }
}