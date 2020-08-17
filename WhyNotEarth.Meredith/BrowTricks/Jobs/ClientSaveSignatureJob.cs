using System.IO;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.GoogleCloud;
using WhyNotEarth.Meredith.HelloSign;
using WhyNotEarth.Meredith.Services;
using WhyNotEarth.Meredith.Twilio;

namespace WhyNotEarth.Meredith.BrowTricks.Jobs
{
    internal class ClientSaveSignatureJob : IClientSaveSignatureJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IDbContext _dbContext;
        private readonly IFileService _fileService;
        private readonly GoogleStorageService _googleStorageService;
        private readonly IHelloSignService _helloSignService;
        private readonly PmuNotifications _pmuNotifications;

        public ClientSaveSignatureJob(IDbContext dbContext, IHelloSignService helloSignService,
            GoogleStorageService googleStorageService, PmuNotifications pmuNotifications,
            IBackgroundJobClient backgroundJobClient, IFileService fileService)
        {
            _dbContext = dbContext;
            _helloSignService = helloSignService;
            _googleStorageService = googleStorageService;
            _pmuNotifications = pmuNotifications;
            _backgroundJobClient = backgroundJobClient;
            _fileService = fileService;
        }

        public async Task SaveSignature(string signatureRequestId)
        {
            var client = await _dbContext.Clients
                    .Include(item => item.Tenant)
                    .Include(item => item.User)
                    .FirstOrDefaultAsync(item => item.SignatureRequestId == signatureRequestId);

            if (client is null)
            {
                return;
            }

            var pdfData = _helloSignService.DownloadSignature(signatureRequestId);

            var path = GetFilePath(client);
            await _googleStorageService.UploadFileAsync(path, "application/pdf", new MemoryStream(pdfData));

            client.PmuPdf = path;
            client.PmuStatus = PmuStatusType.Completed;
            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();

            await SendPmuCompletionNotificationAsync(client);
        }

        private async Task SendPmuCompletionNotificationAsync(Client client)
        {
            if (string.IsNullOrWhiteSpace(client.User.PhoneNumber))
            {
                return;
            }

            var pdfUrl = await _fileService.GetPrivateUrlAsync(client.PmuPdf!);

            var shortMessage = _pmuNotifications.GetCompletionNotification(client.Tenant, client.User, pdfUrl);

            _dbContext.ShortMessages.Add(shortMessage);
            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<ITwilioService>(service =>
                service.SendAsync(shortMessage.Id));
        }

        private string GetFilePath(Client client)
        {
            return string.Join("/", BrowTricksCompany.Slug, "pmu", client.Id.ToString());
        }
    }
}