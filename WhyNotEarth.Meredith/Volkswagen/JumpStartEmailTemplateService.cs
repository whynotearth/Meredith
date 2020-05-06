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

        public string GetEmailHtml(DateTime date, List<Article> articles)
        {
            return GetTemplate(date, articles, EmailTemplateFileName);
        }

        public string GetPdfHtml(DateTime date, List<Article> articles)
        {
            return GetTemplate(date, articles, PdfTemplateFileName);
        }

        private string GetTemplate(DateTime date, List<Article> articles, string templateName)
        {
            var assembly = typeof(JumpStartService).GetTypeInfo().Assembly;

            var name = assembly.GetManifestResourceNames().FirstOrDefault(item => item.EndsWith(templateName));
            var stream = assembly.GetManifestResourceStream(name);

            if (stream is null)
            {
                throw new Exception($"Missing {templateName} resource.");
            }

            var reader = new StreamReader(stream);

            var rawTemplate = reader.ReadToEnd();

            return ReplaceArticles(rawTemplate, "Project Blue Delta", date, articles);
        }

        private string ReplaceArticles(string rawTemplate, string title, DateTime date, List<Article> articles)
        {
            var data = new Dictionary<string, object>();

            RegisterHelpers();

            var template = Handlebars.Compile(rawTemplate);
            
            AddGeneralData(data, title, date);

            var answer = articles.Single(item => item.Category.Slug == AnswersCategorySlug);
            AddArticle("article0", data, answer);

            var top = articles.Where(item => item.Category.Slug == PriorityCategorySlug);
            AddArticles("articlesTop", data, top);

            var middle = articles.Where(item => item.Category.Slug != AnswersCategorySlug && item.Category.Slug != PriorityCategorySlug).Take(2);
            AddArticles("articlesDouble", data, middle);

            var bottom = articles.Where(item => item.Category.Slug != AnswersCategorySlug && item.Category.Slug != PriorityCategorySlug).Skip(2);
            AddArticles("articlesBottom", data, bottom);

            var result = template(data);
            return result;
        }

        private void AddArticles(string key, Dictionary<string, object> data, IEnumerable<Article> articles)
        {
            var items = articles.Select(GetData).ToList();

            data.Add(key, items);
        }

        private void AddArticle(string key, Dictionary<string, object> data, Article article)
        {
            data.Add(key, GetData(article));
        }

        private Dictionary<string, object> GetData(Article article)
        {
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

        private void AddGeneralData(Dictionary<string, object> data, string title, DateTime date)
        {
            data.Add("general", new Dictionary<string, object>
            {
                {"title", title},
                {"date", date.InZone(VolkswagenCompany.TimeZoneId, "dddd | MMM. d, yyyy")}
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
    }
}