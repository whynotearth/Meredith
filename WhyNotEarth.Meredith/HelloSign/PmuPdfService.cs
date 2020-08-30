using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Makrdown;
using WhyNotEarth.Meredith.Pdf;

namespace WhyNotEarth.Meredith.HelloSign
{
    internal class PmuPdfService : IPmuPdfService
    {
        private readonly IHtmlService _htmlService;
        private readonly IMarkdownService _markdownService;

        public PmuPdfService(IHtmlService htmlService, IMarkdownService markdownService)
        {
            _htmlService = htmlService;
            _markdownService = markdownService;
        }

        public Task<byte[]> GetPdfAsync(List<Disclosure> disclosures)
        {
            var templateHtml = GetTemplateHtml(disclosures);

            return _htmlService.ToPdfAsync(templateHtml);
        }

        public Task<byte[]> GetPngAsync(List<Disclosure> disclosures)
        {
            var templateHtml = GetTemplateHtml(disclosures);

            return _htmlService.ToPngAsync(templateHtml);
        }

        private string GetTemplateHtml(List<Disclosure> disclosures)
        {
            const string templateName = "Pmu.html";

            var templateHtml = GetTemplateHtml(templateName);

            templateHtml = AddDisclosures(templateHtml, disclosures);

            return templateHtml;
        }

        private string AddDisclosures(string templateHtml, List<Disclosure> disclosures)
        {
            const string placeHolder = "[_Disclosures_]";

            var result = new StringBuilder();

            foreach (var disclosure in disclosures)
            {
                var disclosureHtml = _markdownService.ToHtml(disclosure.Value);

                result.AppendFormat("<p><span style=\"color: white;\">[initial|req|signer1]</span>{0}</p><br />",
                    disclosureHtml);
            }

            return templateHtml.Replace(placeHolder, result.ToString());
        }

        private string GetTemplateHtml(string templateName)
        {
            var assembly = typeof(PmuPdfService).GetTypeInfo().Assembly;

            var name = assembly.GetManifestResourceNames().FirstOrDefault(item => item.EndsWith(templateName));

            if (name is null)
            {
                throw new Exception($"Missing {templateName} resource.");
            }

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