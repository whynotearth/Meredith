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

        public Task<byte[]> GetPdfAsync(Public.Tenant tenant, List<Disclosure> disclosures)
        {
            var templateHtml = GetTemplateHtml(tenant, disclosures);

            return _htmlService.ToPdfAsync(templateHtml);
        }

        public Task<byte[]> GetPngAsync(Public.Tenant tenant, List<Disclosure> disclosures)
        {
            var templateHtml = GetTemplateHtml(tenant, disclosures);

            return _htmlService.ToPngAsync(templateHtml);
        }

        private string GetTemplateHtml(Public.Tenant tenant, List<Disclosure> disclosures)
        {
            const string templateName = "Pmu.html";

            var templateHtml = GetTemplateHtml(templateName);

            templateHtml = FillTheTemplate(templateHtml, tenant, disclosures);

            return templateHtml;
        }

        private string FillTheTemplate(string templateHtml, Public.Tenant tenant, List<Disclosure> disclosures)
        {
            var keyValues = new Dictionary<string, string>
            {
                {"[_Disclosures_]", GetDisclosures(disclosures)},
                {"[_TenantName_]", tenant.Name}
            };

            foreach (var keyValue in keyValues)
            {
                templateHtml = templateHtml.Replace(keyValue.Key, keyValue.Value);
            }

            return templateHtml;
        }

        private string GetDisclosures(List<Disclosure> disclosures)
        {
            var result = new StringBuilder();

            foreach (var disclosure in disclosures)
            {
                var disclosureHtml = _markdownService.ToHtml(disclosure.Value);

                result.AppendFormat("<p class=\"paragraph\">{0}</p>", disclosureHtml);
            }

            return result.ToString();
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