using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HelloSign;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.GoogleCloud;
using Client = WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks.Client;

namespace WhyNotEarth.Meredith.HelloSign
{
    internal class HelloSignService : IHelloSignService
    {
        private readonly IClientService _clientService;
        private readonly MeredithDbContext _dbContext;
        private readonly GoogleStorageService _googleStorageService;
        private readonly HelloSignOptions _options;

        public HelloSignService(IOptions<HelloSignOptions> options, MeredithDbContext dbContext,
            GoogleStorageService googleStorageService, IClientService clientService)
        {
            _dbContext = dbContext;
            _googleStorageService = googleStorageService;
            _clientService = clientService;
            _options = options.Value;
        }

        public async Task<string> GetSignatureRequestAsync(int clientId, User user)
        {
            var client = await _clientService.GetClientAsync(clientId, user);

            var apiClient = new global::HelloSign.Client(_options.ApiKey);

            var request = new TemplateSignatureRequest();
            request.AddTemplate(_options.TemplateId);
            request.TestMode = _options.TestMode;
            request.AddSigner("Client", client.User.Email, client.User.FullName);

            request.AddCustomField("Name", client.User.FullName);
            request.AddCustomField("TenantName", client.Tenant.Name);
            request.AddCustomField("Conditions", client.Conditions);
            request.AddCustomField("PhysicianName", client.PhysicianName);
            request.AddCustomField("PhysicianPhoneNumber", client.PhysicianPhoneNumber);

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

            if (myEvent.EventType == "signature_request_all_signed")
            {
                await SavePdfAsync(myEvent.SignatureRequest.SignatureRequestId);
            }
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
            client.IsPmuCompleted = true;
            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        private string GetFilePath(Client client)
        {
            return Path.Combine(BrowTricksCompany.Slug, "pmu", client.Id.ToString());
        }
    }
}