using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HandlebarsDotNet;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartEmailTemplateService
    {
        private const string EmailTemplateFileName = "EmailTemplate.html";
        private const string PdfTemplateFileName = "PdfTemplate.html";
        private const string AnswersCategorySlug = "answers-at-a-glance";
        private const string PriorityCategorySlug = "priority";

        public string GetEmailHtml(JumpStart jumpStart, string? pdfUrl)
        {
            return GetTemplate(jumpStart, EmailTemplateFileName, pdfUrl);
        }

        public string GetPdfHtml(JumpStart jumpStart)
        {
            return GetTemplate(jumpStart, PdfTemplateFileName, null);
        }

        private string GetTemplate(JumpStart jumpStart, string templateName, string? pdfUrl)
        {
            var data = GetData(jumpStart, pdfUrl);

            return Compile(templateName, data);
        }

        private string Compile(string templateName, Dictionary<string, object> data)
        {
            RegisterHelpers();

            var rawTemplate = GetRawTemplate(templateName);

            var template = Handlebars.Compile(rawTemplate);

            var result = template(data);

            return result;
        }

        private Dictionary<string, object> GetData(JumpStart jumpStart, string? pdfUrl)
        {
            var result = new Dictionary<string, object>();
            
            AddGeneralData(result, jumpStart.DateTime, pdfUrl);

            var remainingArticles = jumpStart.Articles.ToList();

            var (answer, remains) = PickArticle(remainingArticles, item => item.Category.Slug == AnswersCategorySlug);
            remainingArticles = remains;
            AddArticleToData("article0", result, answer);

            var (top, remaining) = PickArticles(remainingArticles, item => item.Category.Slug == PriorityCategorySlug);
            remainingArticles = remaining;
            AddArticlesToData("articlesTop", result, top);

            var middle = remainingArticles.Take(2);
            AddArticlesToData("articlesDouble", result, middle);

            var bottom = remainingArticles.Skip(2);
            AddArticlesToData("articlesBottom", result, bottom);

            return result;
        }

        private (Article?, List<Article>) PickArticle(List<Article> articles, Func<Article, bool> selector)
        {
            var remainingArticles = new List<Article>();
            Article? result = null;

            foreach (var article in articles)
            {
                if (result == null && selector(article))
                {
                    result = article;
                }
                else
                {
                    remainingArticles.Add(article);
                }
            }

            return (result, remainingArticles);
        }

        private (List<Article>, List<Article>) PickArticles(List<Article> articles, Func<Article, bool> selector)
        {
            var remainingArticles = new List<Article>();
            var selectedArticles = new List<Article>();

            foreach (var article in articles)
            {
                if (selector(article))
                {
                    selectedArticles.Add(article);
                }
                else
                {
                    remainingArticles.Add(article);
                }
            }

            return (selectedArticles, remainingArticles);
        }

        private void AddArticlesToData(string key, Dictionary<string, object> data, IEnumerable<Article> articles)
        {
            var items = articles.Select(GetData).ToList();

            data.Add(key, items);
        }

        private void AddArticleToData(string key, Dictionary<string, object> data, Article? article)
        {
            data.Add(key, GetData(article));
        }

        private Dictionary<string, object> GetData(Article? article)
        {
            if (article is null)
            {
                return new Dictionary<string, object>();
            }

            return new Dictionary<string, object>
            {
                {"id", article.Id},
                {"headline", article.Headline},
                {"description", article.Description},
                {
                    "image", new Dictionary<string, object>
                    {
                        {"url", article.Image?.Url ?? string.Empty}
                    }
                },
                {
                    "category", new Dictionary<string, object>
                    {
                        {"slug", article.Category?.Slug ?? string.Empty},
                        {"name", article.Category?.Name ?? string.Empty},
                        {"color", article.Category?.Color ?? string.Empty},
                        {"image", article.Category?.Image?.Url ?? string.Empty}
                    }
                }
            };
        }

        private void AddGeneralData(Dictionary<string, object> data, DateTime date, string? pdfUrl)
        {
            data.Add("general", new Dictionary<string, object>
            {
                {"title", "Project Blue Delta"},
                {"date", date.InZone(VolkswagenCompany.TimeZoneId, "dddd | MMM. d, yyyy")},
                {"print_url", pdfUrl ?? string.Empty}
            });
        }

        private void RegisterHelpers()
        {
            Handlebars.RegisterHelper("to_slug", (writer, context, parameters) =>
            {
                var slug = ((string) parameters[0]).ToLower();
                var cleanSlug = Regex.Replace(slug, "/\\s/g", "-");
                writer.Write(cleanSlug);
            });

            Handlebars.RegisterHelper("cloudinary_transform", (writer, context, parameters) =>
            {
                var url = (string) parameters[0];
                var transform = (string) parameters[1];

                var value = Regex.Replace(url, "\\/upload\\/", $"/upload/{transform}/");
                writer.Write(value);
            });

            Handlebars.RegisterHelper("equals", (writer, context, parameters) =>
            {
                var arg1 = (string) parameters[0];
                var arg2 = (string) parameters[1];

                writer.Write(arg1 == arg2);
            });

            Handlebars.RegisterHelper("not_equals", (writer, context, parameters) =>
            {
                var arg1 = (string) parameters[0];
                var arg2 = (string) parameters[1];

                writer.Write(arg1 != arg2);
            });
        }

        private string GetRawTemplate(string templateName)
        {
            var assembly = typeof(JumpStartService).GetTypeInfo().Assembly;

            var name = assembly.GetManifestResourceNames().FirstOrDefault(item => item.EndsWith(templateName));
            var stream = assembly.GetManifestResourceStream(name);

            if (stream is null)
            {
                throw new Exception($"Missing {templateName} resource.");
            }

            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}