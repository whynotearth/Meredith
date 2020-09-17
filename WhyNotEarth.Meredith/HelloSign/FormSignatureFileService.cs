using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.BrowTricks.FormWidgets;
using WhyNotEarth.Meredith.Pdf;

namespace WhyNotEarth.Meredith.HelloSign
{
    internal class FormSignatureFileService : IFormSignatureFileService
    {
        private readonly IHtmlService _htmlService;

        public FormSignatureFileService(IHtmlService htmlService)
        {
            _htmlService = htmlService;
        }

        public string GetHtml(FormSignature formSignature)
        {
            var widgets = GetWidgets(formSignature);

            return BuildHtml(formSignature.Name, widgets, true, formSignature.Client.User.FullName,
                formSignature.CreatedAt);
        }

        public async Task<byte[]> GetPngAsync(FormTemplate formTemplate)
        {
            var widgets = GetWidgets(formTemplate);

            var html = BuildHtml(formTemplate.Name, widgets, false, null, null);

            return await _htmlService.ToPngAsync(html);
        }

        public async Task<byte[]> GetPngAsync(FormSignature formSignature)
        {
            var widgets = GetWidgets(formSignature);

            var html = BuildHtml(formSignature.Name, widgets, true, formSignature.Client.User.FullName,
                formSignature.CreatedAt);

            return await _htmlService.ToPngAsync(html);
        }

        private string BuildHtml(string tenantName, List<IFormWidget> widgets, bool hasAnswers, string? clientName,
            DateTime? dateTime)
        {
            var template = GetTemplate("Pmu.html");

            var body = GetBody(widgets);

            var keyValues = new Dictionary<string, string>
            {
                {"{title}", tenantName},
                {"{bodyClasses}", hasAnswers ? "has-answers" : string.Empty},
                {"{body}", body},
                {"{signature}", GetSignature(hasAnswers, clientName, dateTime)}
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

        private string GetSignature(bool hasAnswers, string? name, DateTime? dateTime)
        {
            if (!hasAnswers)
            {
                return string.Empty;
            }

            return $@"
<section class=""section"">
    <hr />
</section>
<section class=""section"">
    <p class=""font-light mb-1"">
        Signed by <span class=""font-normal"">{name}</span>
    </p>
    <p class=""font-light"">{dateTime!.Value:d MMM, yyyy}</p>
</section>";
        }

        private List<IFormWidget> GetWidgets(FormTemplate formTemplate)
        {
            var result = new List<IFormWidget>();

            if (formTemplate.Items is null)
            {
                return result;
            }

            foreach (var formItem in formTemplate.Items)
            {
                IFormWidget formWidget = formItem.Type switch
                {
                    FormItemType.Text => new TextFormWidget(formItem),
                    FormItemType.AgreementRequest => new AgreementRequestFormWidget(formItem),
                    FormItemType.TextResponse => new TextResponseFormWidget(formItem),
                    FormItemType.Checklist => new ChecklistFormWidget(formItem),
                    FormItemType.MultipleChoice => new MultipleChoiceFormWidget(formItem),
                    FormItemType.Image => new ImageFormWidget(formItem),
                    FormItemType.Pdf => new PdfFormWidget(formItem),
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
                IFormWidget formWidget = answer.Type switch
                {
                    FormItemType.Text => new TextFormWidget(answer),
                    FormItemType.AgreementRequest => new AgreementRequestFormWidget(answer),
                    FormItemType.TextResponse => new TextResponseFormWidget(answer),
                    FormItemType.Checklist => new ChecklistFormWidget(answer),
                    FormItemType.MultipleChoice => new MultipleChoiceFormWidget(answer),
                    FormItemType.Image => new ImageFormWidget(answer),
                    FormItemType.Pdf => new PdfFormWidget(answer),
                    _ => throw new NotSupportedException()
                };

                result.Add(formWidget);
            }

            return result;
        }

        private string GetTemplate(string templateName)
        {
            var assembly = typeof(FormSignatureFileService).GetTypeInfo().Assembly;

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