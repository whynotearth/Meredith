using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Markdig;
using WhyNotEarth.Meredith.Pdf;

namespace WhyNotEarth.Meredith.Makrdown
{
    internal class MarkdownService : IMarkdownService
    {
        private readonly IHtmlService _htmlService;

        public MarkdownService(IHtmlService htmlService)
        {
            _htmlService = htmlService;
        }

        [return: NotNullIfNotNull("markdown")]
        public string ToHtml(string markdown)
        {
            return Markdown.ToHtml(markdown);
        }

        public Task<byte[]> ToPdfAsync(string markdown)
        {
            var html = ToHtml(markdown);

            return _htmlService.ToPdfAsync(html);
        }
    }
}