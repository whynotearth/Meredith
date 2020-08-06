using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelloSign;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.GoogleCloud;
using Client = WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks.Client;

namespace WhyNotEarth.Meredith.HelloSign
{
    internal class HelloSignService : IHelloSignService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly GoogleStorageService _googleStorageService;
        private readonly HelloSignOptions _options;

        public HelloSignService(IOptions<HelloSignOptions> options, MeredithDbContext dbContext,
            GoogleStorageService googleStorageService)
        {
            _dbContext = dbContext;
            _googleStorageService = googleStorageService;
            _options = options.Value;
        }

        async Task<string> IHelloSignService.GetSignatureRequestAsync(int clientId)
        {
            var client = await GetClientAsync(clientId);

            var apiClient = new global::HelloSign.Client(_options.ApiKey);

            var request = new TemplateSignatureRequest();
            request.AddTemplate(_options.TemplateId);
            request.TestMode = _options.TestMode;
            request.AddSigner("Client", client.User.Email, client.User.FullName);

            request.AddCustomField("Name", client.User.FullName);
            request.AddCustomField("TenantName", client.Tenant.Name);
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

        public async Task ProcessEventsAsync(string json)
        {
            var apiClient = new global::HelloSign.Client(_options.ApiKey);
            var myEvent = apiClient.ParseEvent(json);

            if (myEvent.EventType == "signature_request_downloadable")
            {
                await SavePdfAsync(myEvent.SignatureRequest.SignatureRequestId);
            }
        }

        private async Task AddDisclosuresAsync(TemplateSignatureRequest request, Client client)
        {
            var disclosures = await _dbContext.Disclosures.Where(item => item.ClientId == client.Id)
                .ToListAsync();

            var result = new StringBuilder();
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

        private async Task SavePdfAsync(string signatureRequestId)
        {
            var client =
                await _dbContext.Clients.FirstOrDefaultAsync(item => item.SignatureRequestId == signatureRequestId);

            if (client is null)
            {
                return;
            }

            var apiClient = new global::HelloSign.Client(_options.ApiKey);

            var pdfData = apiClient.DownloadSignatureRequestFiles(client.SignatureRequestId);

            var path = GetFilePath(client);
            await _googleStorageService.UploadFileAsync(path, "application/pdf", new MemoryStream(pdfData));

            client.PmuPdf = path;
            client.PmuStatus = PmuStatusType.Completed;
            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        private string GetFilePath(Client client)
        {
            return string.Join("/", BrowTricksCompany.Slug, "pmu", client.Id.ToString());
        }

        private async Task<Client> GetClientAsync(int clientId)
        {
            var client = await _dbContext.Clients
                .Include(item => item.User)
                .FirstOrDefaultAsync(item => item.Id == clientId);

            if (client is null)
            {
                throw new RecordNotFoundException($"client {clientId} not found");
            }

            return client;
        }
    }
}