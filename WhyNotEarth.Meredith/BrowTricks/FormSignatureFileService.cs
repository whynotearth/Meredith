using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks.FormWidgets;
using WhyNotEarth.Meredith.Pdf;
using WhyNotEarth.Meredith.Services;

namespace WhyNotEarth.Meredith.BrowTricks
{
    internal class FormSignatureFileService : IFormSignatureFileService
    {
        private readonly IHtmlService _htmlService;
        private readonly IResourceService _resourceService;

        public FormSignatureFileService(IHtmlService htmlService, IResourceService resourceService)
        {
            _htmlService = htmlService;
            _resourceService = resourceService;
        }

        public string GetHtml(FormSignature formSignature)
        {
            var widgets = GetWidgets(formSignature);

            return BuildHtml(formSignature.Name, widgets, true, formSignature.Client.FullName,
                formSignature.CreatedAt, formSignature.SignatureImage);
        }

        public async Task<byte[]> GetPngAsync(FormTemplate formTemplate)
        {
            var widgets = GetWidgets(formTemplate);

            var html = BuildHtml(formTemplate.Name, widgets, false, null, null, null);

            return await _htmlService.ToPngAsync(html);
        }

        public async Task<byte[]> GetPdfAsync(FormTemplate formTemplate)
        {
            var widgets = GetWidgets(formTemplate);

            var html = BuildHtml(formTemplate.Name, widgets, false, null, null, null);

            return await _htmlService.ToPdfAsync(html);
        }

        public async Task<byte[]> GetPngAsync(FormSignature formSignature, string fullName)
        {
            var widgets = GetWidgets(formSignature);

            var html = BuildHtml(formSignature.Name, widgets, true, fullName, formSignature.CreatedAt,
                formSignature.SignatureImage);

            return await _htmlService.ToPngAsync(html);
        }

        private string BuildHtml(string tenantName, List<IFormWidget> widgets, bool hasAnswers, string? clientName,
            DateTime? dateTime, string? signatureImage)
        {
            var body = GetBody(widgets);

            var replaceValues = new Dictionary<string, string>
            {
                {"{{title}}", tenantName},
                {"{{bodyClasses}}", hasAnswers ? "has-answers" : string.Empty},
                {"{{body}}", body},
                {"{{has_signature}}", hasAnswers ? string.Empty : "style=\"display: none;\""},
                {"{{signature_image}}", signatureImage ?? string.Empty},
                {"{{signature_name}}", clientName ?? string.Empty},
                {"{{signature_date}}", dateTime?.ToString("d MMM, yyyy") ?? string.Empty},
            };

            var template = _resourceService.Get("Pmu.html", replaceValues);

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
    }
}