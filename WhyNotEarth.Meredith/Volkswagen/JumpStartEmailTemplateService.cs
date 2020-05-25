using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HandlebarsDotNet;
using Markdig;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartEmailTemplateService
    {
        private const string TwoColumnTemplateFileName = "two-column.html";
        private const string TwoColumnPdfTemplateFileName = "two-column-pdf.html";
        private const string ThreeColumnTemplateFileName = "three-column.html";
        private const string ThreeColumnPdfTemplateFileName = "three-column-pdf.html";
        private const string AnswersCategorySlug = "answers-at-a-glance";
        private const string PriorityCategorySlug = "priority";

        public string GetEmailHtml(DateTime date, List<Article> articles, string? pdfUrl)
        {
            var (templateName, isTwoColumn) = GetTemplateName(articles, false);

            return GetTemplate(date, articles, templateName, isTwoColumn, pdfUrl);
        }

        public string GetPdfHtml(DateTime dateTime, List<Article> articles)
        {
            var (templateName, isTwoColumn) = GetTemplateName(articles, true);

            return GetTemplate(dateTime.Date, articles, templateName, isTwoColumn, null);
        }

        private string GetTemplate(DateTime date, List<Article> articles, string templateName, bool isTwoColumn,
            string? pdfUrl)
        {
            var data = GetData(date, articles, isTwoColumn, pdfUrl);

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

        private Dictionary<string, object> GetData(DateTime date, List<Article> articles, bool isTwoColumn,
            string? pdfUrl)
        {
            var result = new Dictionary<string, object>();
            var maxMiddleCount = isTwoColumn ? 2 : 3;

            AddGeneralData(result, date, pdfUrl);

            var remainingArticles = articles.ToList();

            var (answer, remains) = PickArticle(remainingArticles, item => item.Category.Slug == AnswersCategorySlug);
            remainingArticles = remains;
            AddArticleToData("article0", result, answer);

            var (top, remaining) = PickArticles(remainingArticles, item => item.Category.Slug == PriorityCategorySlug);
            remainingArticles = remaining;
            AddArticlesToData("articlesTop", result, top, isTwoColumn, true);

            var middle = remainingArticles.Take(maxMiddleCount).ToList();
            AddArticlesToData("articlesMiddle", result, middle, isTwoColumn, false);

            var bottom = remainingArticles.Skip(maxMiddleCount);
            AddArticlesToData("articlesBottom", result, bottom, isTwoColumn, true);

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

        private void AddArticlesToData(string key, Dictionary<string, object> data, IEnumerable<Article> articles,
            bool isTwoColumn, bool isWide)
        {
            var items = articles.Select(item => GetData(item, isTwoColumn, isWide)).ToList();

            data.Add(key, items);
        }

        private void AddArticleToData(string key, Dictionary<string, object> data, Article? article)
        {
            data.Add(key, GetData(article, false, false));
        }

        private Dictionary<string, object> GetData(Article? article, bool isTwoColumn, bool isWide)
        {
            if (article is null)
            {
                return new Dictionary<string, object>();
            }

            var (width, height, wrapperWidth) = GetImageSize(article.Image, isTwoColumn, isWide);

            return new Dictionary<string, object>
            {
                {"id", article.Id},
                {"headline", article.Headline},
                {"description", RenderMarkdown(article.Description)},
                {"imageCaption", article.ImageCaption},
                {
                    "image", new Dictionary<string, object>
                    {
                        {"url", article.Image?.Url ?? string.Empty},
                        {"width", width},
                        {"height", height},
                        {"wrapperWidth", wrapperWidth}
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
                {"date", date.ToString("dddd | MMM. d, yyyy")},
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
        }

        private string GetRawTemplate(string templateName)
        {
            var assembly = typeof(JumpStartEmailTemplateService).GetTypeInfo().Assembly;

            var name = assembly.GetManifestResourceNames().FirstOrDefault(item => item.EndsWith(templateName));
            var stream = assembly.GetManifestResourceStream(name);

            if (stream is null)
            {
                throw new Exception($"Missing {templateName} resource.");
            }

            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        private (string templateName, bool isTwoColumn) GetTemplateName(List<Article> articles, bool isPdf)
        {
            var hasAnswer = articles.Any(item => item.Category.Slug == AnswersCategorySlug);

            return (hasAnswer, isPdf) switch
            {
                (true, true) => (TwoColumnPdfTemplateFileName, true),
                (true, false) => (TwoColumnTemplateFileName, true),
                (false, true) => (ThreeColumnPdfTemplateFileName, false),
                (false, false) => (ThreeColumnTemplateFileName, false)
            };
        }

        private (int? width, int? height, int wrapperWidth) GetImageSize(Image image, bool isTwoColumn, bool isWide)
        {
            if (image?.Width is null || image.Height is null)
            {
                return default;
            }

            const int twoColumnWideMaximumWidth = 200;
            const int twoColumnMiddleMaximumWidth = 185;
            const int threeColumnWideMaximumWidth = 280;
            const int threeColumnMiddleMaximumWidth = 163;

            const int retinaRatio = 2;

            var maxWidth = (isTwoColumn, isWide) switch
            {
                (true, true) => twoColumnWideMaximumWidth,
                (true, false) => twoColumnMiddleMaximumWidth,
                (false, true) => threeColumnWideMaximumWidth,
                (false, false) => threeColumnMiddleMaximumWidth
            };

            var width = Math.Min(image.Width.Value / retinaRatio, maxWidth);
            var height = (int) ((double) width / image.Width.Value * image.Height.Value);

            return (width, height, maxWidth);
        }

        private string RenderMarkdown(string value)
        {
            return Markdown.ToHtml(value);
        }
    }
}