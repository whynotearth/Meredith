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

        public string GetEmailHtml(DateTime date, List<Post> posts)
        {
            var assembly = typeof(JumpStartService).GetTypeInfo().Assembly;

            var name = assembly.GetManifestResourceNames().FirstOrDefault(item => item.EndsWith(EmailTemplateFileName));
            var stream = assembly.GetManifestResourceStream(name);

            if (stream is null)
            {
                throw new Exception($"Missing {EmailTemplateFileName} resource.");
            }

            var reader = new StreamReader(stream);

            var rawTemplate = reader.ReadToEnd();

            return ReplacePosts(rawTemplate, "Project Blue Delta", date, posts);
        }

        private string ReplacePosts(string rawTemplate, string title, DateTime date, List<Post> posts)
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

            var template = Handlebars.Compile(rawTemplate);

            var dictionary = new Dictionary<string, object>
            {
                {
                    "general", new Dictionary<string, object>
                    {
                        {"title", title},
                        {"date", date.ToString("dddd | MMM. d, yyyy")}
                    }
                }
            };

            for (var i = 0; i < posts.Count; i++)
            {
                var item = posts[i];
                dictionary.Add($"article{i}", new Dictionary<string, object>
                {
                    {"id", item.Id},
                    {"headline", item.Headline},
                    {"description", item.Description},
                    {
                        "image", new Dictionary<string, object>
                        {
                            {"url", item.Image?.Url ?? string.Empty}
                        }
                    },
                    {
                        "category", new Dictionary<string, object>
                        {
                            {"name", item.Category?.Name ?? string.Empty},
                            {"color", item.Category?.Color ?? string.Empty},
                            {"image", item.Category?.Image?.Url ?? string.Empty}
                        }
                    }
                });
            }

            var result = template(dictionary);
            return result;
        }
    }
}