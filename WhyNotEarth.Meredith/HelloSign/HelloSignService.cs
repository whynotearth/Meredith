using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HelloSign;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.GoogleCloud;
using Client = WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks.Client;

namespace WhyNotEarth.Meredith.HelloSign
{
    internal class HelloSignService : IHelloSignService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly GoogleStorageService _googleStorageService;
        private readonly HelloSignOptions _options;

        public HelloSignService(IOptions<HelloSignOptions> options, IWebHostEnvironment environment,
            MeredithDbContext dbContext, GoogleStorageService googleStorageService)
        {
            _environment = environment;
            _dbContext = dbContext;
            _googleStorageService = googleStorageService;
            _options = options.Value;
        }

        public async Task<string> GetSignatureRequestAsync(string tenantSlug, User user)
        {
            var client = await GetClientAsync(tenantSlug, user);

            if (client is null)
            {
                throw new RecordNotFoundException($"Client for user {user.Id} not found");
            }

            var apiClient = new global::HelloSign.Client(_options.ApiKey);

            var request = new TemplateSignatureRequest();
            request.AddTemplate(_options.TemplateId);
            request.TestMode = _environment.IsDevelopment();
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
            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        private string GetFilePath(Client client)
        {
            return Path.Combine(BrowTricksCompany.Slug, "pmu", client.Id.ToString());
        }

        private Task<Client> GetClientAsync(string tenantSlug, User user)
        {
            return _dbContext.Clients
                .Include(item => item.User)
                .Include(item => item.Tenant)
                .FirstOrDefaultAsync(item => item.UserId == user.Id && item.Tenant.Slug == tenantSlug);
        }
    }
}