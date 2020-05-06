using System;
using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class JumpStartResult
    {
        public DateTime Date { get; }

        public List<ArticleResult> Articles { get; }

        public JumpStartResult(JumpStart jumpStart)
        {
            Date = jumpStart.DateTime;
            Articles = jumpStart.Articles.Select(item => new ArticleResult(item)).ToList() ?? new List<ArticleResult>();
        }
    }

    public class ArticleResult
    {
        public string CategoryImage { get; }

        public string Headline { get; }

        public ArticleResult(Article article)
        {
            CategoryImage = article.Category.Image.Url;
            Headline = article.Headline;
        }
    }
}