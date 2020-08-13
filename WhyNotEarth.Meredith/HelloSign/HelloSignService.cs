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
using WhyNotEarth.Meredith.Public;
using Client = WhyNotEarth.Meredith.BrowTricks.Client;

namespace WhyNotEarth.Meredith.HelloSign
{
    internal class HelloSignService : IHelloSignService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly HelloSignOptions _options;

        public HelloSignService(IOptions<HelloSignOptions> options, MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
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
            request.AddFile(GetTemplate(disclosures.Any()), "pmu.pdf", "application/pdf");

            // https://github.com/hellosign/hellosign-dotnet-sdk/issues/59
            var customFields = new List<CustomField>
            {
                new CustomField("Name", user.FullName),
                new CustomField("TenantName", tenant.Name)
            };
            AddDisclosures(customFields, disclosures);
            apiClient.AdditionalParameters.Add("custom_fields", JsonSerializer.Serialize(customFields));

            var embeddedSignatureResponse =
                apiClient.CreateEmbeddedSignatureRequest(request, _options.ClientId);
            var signatureId = embeddedSignatureResponse.Signatures.First().SignatureId;

            apiClient.AdditionalParameters.Remove("custom_fields");
            var signUrlResponse = apiClient.GetSignUrl(signatureId);

            client.SignatureRequestId = embeddedSignatureResponse.SignatureRequestId;
            _dbContext.Update(client);
            await _dbContext.SaveChangesAsync();

            return signUrlResponse.SignUrl;
        }

        private void AddDisclosures(List<CustomField> customFields, List<Disclosure> disclosures)
        {
            if (!disclosures.Any())
            {
                return;
            }

            var result = new StringBuilder();

            foreach (var disclosure in disclosures)
            {
                result.AppendFormat("{0}\r\n\r\n", disclosure.Value);
            }

            customFields.Add(new CustomField("Disclosures", result.ToString()));
        }

        private byte[] GetTemplate(bool hasDisclosures)
        {
            // https://app.hellosign.com/api/textTagsWalkthrough
            var templateName = hasDisclosures ? "PmuWithDisclosures.pdf" : "Pmu.pdf";

            var assembly = typeof(HelloSignService).GetTypeInfo().Assembly;

            var name = assembly.GetManifestResourceNames().FirstOrDefault(item => item.EndsWith(templateName));
            var stream = assembly.GetManifestResourceStream(name);

            if (stream is null)
            {
                throw new Exception($"Missing {templateName} resource.");
            }

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
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