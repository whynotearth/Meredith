using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelloSign;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using Client = WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks.Client;

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

        async Task<string> IHelloSignService.GetSignatureRequestAsync(Client client, User user,
            Data.Entity.Models.Tenant tenant)
        {
            var apiClient = new global::HelloSign.Client(_options.ApiKey);

            var request = new TemplateSignatureRequest();
            request.AddTemplate(_options.TemplateId);
            request.TestMode = _options.TestMode;
            request.AddSigner("Client", user.Email, user.FullName);

            request.AddCustomField("Name", user.FullName);
            request.AddCustomField("TenantName", tenant.Name);
            await AddDisclosuresAsync(request, client);

            var embeddedSignatureResponse =
                apiClient.CreateEmbeddedSignatureRequest(request, _options.ClientId);
            var signatureId = embeddedSignatureResponse.Signatures.First().SignatureId;

            var signUrlResponse = apiClient.GetSignUrl(signatureId);

            client.SignatureRequestId = embeddedSignatureResponse.SignatureRequestId;
            _dbContext.Update(client);
            await _dbContext.SaveChangesAsync();

            return signUrlResponse.SignUrl;
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

        private async Task AddDisclosuresAsync(TemplateSignatureRequest request, Client client)
        {
            var disclosures = await _dbContext.Disclosures.Where(item => item.ClientId == client.Id)
                .ToListAsync();

            var result = new StringBuilder();

            if (disclosures.Any())
            {
                result.AppendFormat("{0}\r\n",
                    "Your artist has added some additional disclosures. Please read and sign if you agree to book your appointment");
            }

            foreach (var disclosure in disclosures)
            {
                result.AppendFormat("{0}\r\n\r\n", disclosure.Value);
            }

            request.CustomFields.Add(new CustomField
            {
                Value = result.ToString(),
                Name = "Disclosures"
            });
        }
    }
}