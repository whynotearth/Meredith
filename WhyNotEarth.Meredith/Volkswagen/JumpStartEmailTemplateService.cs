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

        public string GetEmailHtml(DateTime date, List<Post> posts)
        {
            return GetTemplate(date, posts, EmailTemplateFileName);
        }

        public string GetPdfHtml(DateTime date, List<Post> posts)
        {
            return GetTemplate(date, posts, PdfTemplateFileName);
        }

        private string GetTemplate(DateTime date, List<Post> posts, string templateName)
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

            return ReplacePosts(rawTemplate, "Project Blue Delta", date, posts);
        }

        private string ReplacePosts(string rawTemplate, string title, DateTime date, List<Post> posts)
        {
            var data = new Dictionary<string, object>();

            RegisterHelpers();

            var template = Handlebars.Compile(rawTemplate);
            
            AddGeneralData(data, title, date);

            var answer = posts.Single(item => item.Category.Slug == AnswersCategorySlug);
            AddPost("article0", data, answer);

            var top = posts.Where(item => item.Category.Slug == PriorityCategorySlug);
            AddPosts("articlesTop", data, top);

            var middle = posts.Where(item => item.Category.Slug != AnswersCategorySlug && item.Category.Slug != PriorityCategorySlug).Take(2);
            AddPosts("articlesDouble", data, middle);

            var bottom = posts.Where(item => item.Category.Slug != AnswersCategorySlug && item.Category.Slug != PriorityCategorySlug).Skip(2);
            AddPosts("articlesBottom", data, bottom);

            var result = template(data);
            return result;
        }

        private void AddPosts(string key, Dictionary<string, object> data, IEnumerable<Post> posts)
        {
            var items = posts.Select(GetData).ToList();

            data.Add(key, items);
        }

        private void AddPost(string key, Dictionary<string, object> data, Post post)
        {
            data.Add(key, GetData(post));
        }

        private Dictionary<string, object> GetData(Post post)
        {
            return new Dictionary<string, object>
            {
                {"id", post.Id},
                {"headline", post.Headline},
                {"description", post.Description},
                {
                    "image", new Dictionary<string, object>
                    {
                        {"url", post.Image?.Url ?? string.Empty}
                    }
                },
                {
                    "category", new Dictionary<string, object>
                    {
                        {"slug", post.Category?.Slug ?? string.Empty},
                        {"name", post.Category?.Name ?? string.Empty},
                        {"color", post.Category?.Color ?? string.Empty},
                        {"image", post.Category?.Image?.Url ?? string.Empty}
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