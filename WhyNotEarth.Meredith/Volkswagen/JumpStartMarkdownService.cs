using System.Collections.Generic;
using System.IO;
using System.Text;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartMarkdownService
    {
        public string Render(string value)
        {
            var markdownDocument = Markdown.Parse(value);

            Walk(markdownDocument);

            var builder = new StringBuilder();
            var textWriter = new StringWriter(builder);

            var renderer = new HtmlRenderer(textWriter);
            renderer.Render(markdownDocument);

            return builder.ToString();
        }

        private void Walk(MarkdownObject markdownObject)
        {
            foreach (var child in markdownObject.Descendants())
            {
                if (child is ParagraphBlock paragraphBlock)
                {
                    AddClass(paragraphBlock, "body-1", "bb-8");
                    AddStyle(paragraphBlock,
                        "Margin:0;Margin-bottom:10px;border-bottom:8px solid #fff;color:#000;font-family:Arial,sans-serif;font-size:16px;font-weight:400;line-height:20px;margin:0;margin-bottom:0;padding:0;text-align:left");
                }
                else if (child is LinkInline linkInline && !linkInline.IsImage)
                {
                    AddClass(linkInline, "body-1", "link-text");
                    AddStyle(linkInline,
                        "Margin:0;color:#1972b3;font-family:Arial,sans-serif;font-size:16px;font-weight:400;line-height:20px;margin:0;margin-bottom:0;padding:0;text-align:left;text-decoration:none");
                }
                else if (child is ListBlock listBlock)
                {
                    AddClass(listBlock, "body-1", "bb-8", "m-0", "p-0");
                    AddStyle(listBlock,
                        "border-bottom:8px solid #fff;font-size:16px;font-weight:400;line-height:20px;list-style-position:inside;margin:0!important;padding:0!important");
                }
                else if (child is ListItemBlock listItemBlock)
                {
                    AddClass(listItemBlock, "black-text");
                    AddStyle(listItemBlock, "color:#000;font-size:16px;font-weight:400;line-height:20px");
                }
                else if (child is HeadingBlock headingBlock && headingBlock.Level == 2)
                {
                    AddClass(headingBlock, "h2", "bb-16", "m-0");
                    AddStyle(headingBlock,
                        "Margin:0;Margin-bottom:10px;border-bottom:16px solid #fff;color:inherit;font-family:Arial,sans-serif;font-size:22px;font-weight:700;line-height:27px;margin:0!important;margin-bottom:10px;padding:0;text-align:left;word-wrap:normal");
                }
                else if (child is EmphasisInline emphasisInline)
                {
                    AddClass(emphasisInline, "body-1-em-high", "link-text");
                    AddStyle(emphasisInline, "color:#1972b3;font-size:16px;font-weight:700;line-height:20px");
                }
            }
        }

        private void AddClass(IMarkdownObject markdownObject, params string[] classes)
        {
            var attributes = markdownObject.GetAttributes() ?? new HtmlAttributes
            {
                Classes = new List<string>()
            };

            attributes.Classes ??= new List<string>();

            attributes.Classes.AddRange(classes);

            markdownObject.SetAttributes(attributes);
        }

        private void AddStyle(IMarkdownObject markdownObject, string style)
        {
            var attributes = markdownObject.GetAttributes() ?? new HtmlAttributes
            {
                Properties = new List<KeyValuePair<string, string>>()
            };

            attributes.Properties ??= new List<KeyValuePair<string, string>>();

            AddStyle(attributes.Properties, style);

            markdownObject.SetAttributes(attributes);
        }

        private void AddStyle(List<KeyValuePair<string, string>> properties, string style)
        {
            int? index = null;

            for (var i = 0; i < properties.Count; i++)
            {
                if (properties[i].Key == "style")
                {
                    index = i;
                }
            }

            if (index is null)
            {
                properties.Add(new KeyValuePair<string, string>("style", style));
            }
            else
            {
                var updatedStyle = AppendStyle(properties[index.Value].Value, style);
                properties[index.Value] = new KeyValuePair<string, string>("style", updatedStyle);
            }
        }

        private string AppendStyle(string oldValue, string newValue)
        {
            if (string.IsNullOrWhiteSpace(oldValue))
            {
                return newValue;
            }

            if (oldValue.EndsWith(";"))
            {
                return oldValue + newValue;
            }

            return $"{oldValue};{newValue}";
        }
    }
}