using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HelloSign;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Makrdown;
using WhyNotEarth.Meredith.Pdf;
using WhyNotEarth.Meredith.Public;
using Client = WhyNotEarth.Meredith.BrowTricks.Client;

namespace WhyNotEarth.Meredith.HelloSign
{
    internal class HelloSignService : IHelloSignService
    {
        // https://app.hellosign.com/api/textTagsWalkthrough
        // https://app.hellosign.com/api/embeddedTest

        private readonly IDbContext _dbContext;
        private readonly IHtmlService _htmlService;
        private readonly IMarkdownService _markdownService;
        private readonly HelloSignOptions _options;

        public HelloSignService(IOptions<HelloSignOptions> options, IDbContext dbContext, IHtmlService htmlService,
            IMarkdownService markdownService)
        {
            _dbContext = dbContext;
            _htmlService = htmlService;
            _markdownService = markdownService;
            _options = options.Value;
        }

        public string? GetDownloadableSignaturesAsync(string json)
        {
            var apiClient = new global::HelloSign.Client(_options.ApiKey);
            var myEvent = apiClient.ParseEvent(json);

            if (myEvent.EventType == "signature_request_downloadable")
            {
                return myEvent.SignatureRequest.SignatureRequestId;
            }

            return null;
        }

        public byte[] DownloadSignature(string signatureRequestId)
        {
            var apiClient = new global::HelloSign.Client(_options.ApiKey);

            return apiClient.DownloadSignatureRequestFiles(signatureRequestId);
        }

        async Task<string> IHelloSignService.GetSignatureRequestAsync(Client client, User user,
            Public.Tenant tenant)
        {
            var disclosures = await _dbContext.Disclosures.Where(item => item.ClientId == client.Id)
                .ToListAsync();

            var apiClient = new global::HelloSign.Client(_options.ApiKey);

            var request = new SignatureRequest
            {
                TestMode = _options.TestMode,
                UseTextTags = true,
                HideTextTags = true
            };

            request.AddSigner(user.Email, user.FullName);
            request.AddFile(await GetTemplateAsync(disclosures), "pmu.pdf", "application/pdf");

            // https://github.com/hellosign/hellosign-dotnet-sdk/issues/59
            var customFields = new List<CustomField>
            {
                new CustomField("Name", user.FullName),
                new CustomField("TenantName", tenant.Name)
            };
            apiClient.AdditionalParameters.Add("custom_fields", JsonSerializer.Serialize(customFields));

            var embeddedSignatureResponse =
                apiClient.CreateEmbeddedSignatureRequest(request, _options.ClientId);
            var signatureId = embeddedSignatureResponse.Signatures.First().SignatureId;

            apiClient.AdditionalParameters.Remove("custom_fields");
            var signUrlResponse = apiClient.GetSignUrl(signatureId);

            client.SignatureRequestId = embeddedSignatureResponse.SignatureRequestId;
            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();

            return signUrlResponse.SignUrl;
        }

        private Task<byte[]> GetTemplateAsync(List<Disclosure> disclosures)
        {
            const string templateName = "Pmu.html";

            var templateHtml = GetTemplateHtml(templateName);

            templateHtml = AddDisclosures(templateHtml, disclosures);

            return _htmlService.ToPdfAsync(templateHtml);
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
            var assembly = typeof(HelloSignService).GetTypeInfo().Assembly;

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

        private class CustomField
        {
            [JsonPropertyName("name")]
            public string Name { get; }

            [JsonPropertyName("value")]
            public string Value { get; }

            public CustomField(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}