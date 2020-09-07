using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.BrowTricks.FormWidgets;
using WhyNotEarth.Meredith.BrowTricks.Services;
using WhyNotEarth.Meredith.Pdf;

namespace WhyNotEarth.Meredith.HelloSign
{
    internal class PmuPdfService : IPmuPdfService
    {
        private readonly IFormTemplateService _formTemplateService;
        private readonly IHtmlService _htmlService;

        public PmuPdfService(IHtmlService htmlService, IFormTemplateService formTemplateService)
        {
            _htmlService = htmlService;
            _formTemplateService = formTemplateService;
        }

        public string GetHtml(FormSignature formSignature)
        {
            var widgets = GetWidgets(formSignature);

            return BuildHtml(formSignature.Name, widgets);
        }

        public async Task<byte[]> GetPngAsync(Public.Tenant tenant)
        {
            var formTemplate = await _formTemplateService.GetAsync(tenant, FormTemplateType.Disclosure);

            var widgets = GetWidgets(formTemplate);

            var html = BuildHtml(formTemplate.Name, widgets);

            return await _htmlService.ToPngAsync(html);
        }

        private string BuildHtml(string name, List<IFormWidget> widgets)
        {
            var template = GetTemplate("Pmu.html");

            var body = GetBody(widgets);

            var keyValues = new Dictionary<string, string>
            {
                {"{title}", name},
                {"{body}", body}
            };

            foreach (var keyValue in keyValues)
            {
                template = template.Replace(keyValue.Key, keyValue.Value);
            }

            return template;
        }

        private string GetBody(List<IFormWidget> formWidgets)
        {
            var result = new StringBuilder();

            foreach (var formWidget in formWidgets)
            {
                result.Append(formWidget.Render());
            }

            return result.ToString();
        }

        private List<IFormWidget> GetWidgets(FormTemplate formTemplate)
        {
            var result = new List<IFormWidget>();

            foreach (var formItem in formTemplate.Items)
            {
                var formWidget = formItem.Type switch
                {
                    FormItemType.Text => new TextFormWidget(formItem),
                    FormItemType.AgreementRequest => new TextFormWidget(formItem),
                    FormItemType.TextResponse => new TextFormWidget(formItem),
                    FormItemType.Checklist => new TextFormWidget(formItem),
                    FormItemType.MultipleChoice => new TextFormWidget(formItem),
                    FormItemType.Image => new TextFormWidget(formItem),
                    FormItemType.Pdf => new TextFormWidget(formItem),
                    _ => throw new NotSupportedException()
                };

                result.Add(formWidget);
            }

            return result;
        }

        private List<IFormWidget> GetWidgets(FormSignature formSignature)
        {
            var result = new List<IFormWidget>();

            foreach (var answer in formSignature.Answers)
            {
                var formWidget = answer.Type switch
                {
                    FormItemType.Text => new TextFormWidget(answer),
                    FormItemType.AgreementRequest => new TextFormWidget(answer),
                    FormItemType.TextResponse => new TextFormWidget(answer),
                    FormItemType.Checklist => new TextFormWidget(answer),
                    FormItemType.MultipleChoice => new TextFormWidget(answer),
                    FormItemType.Image => new TextFormWidget(answer),
                    FormItemType.Pdf => new TextFormWidget(answer),
                    _ => throw new NotSupportedException()
                };

                result.Add(formWidget);
            }

            return result;
        }

        private string GetTemplate(string templateName)
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