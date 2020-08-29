using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
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
        private readonly PmuNotifications _pmuNotifications;
        private readonly IPmuPdfService _pmuPdfService;

        public ClientSaveSignatureJob(IDbContext dbContext, IPmuPdfService pmuPdfService, PmuNotifications pmuNotifications,
            IBackgroundJobClient backgroundJobClient, IFileService fileService)
        {
            _dbContext = dbContext;
            _pmuPdfService = pmuPdfService;
            _pmuNotifications = pmuNotifications;
            _backgroundJobClient = backgroundJobClient;
            _fileService = fileService;
        }

        public async Task SaveSignature(int clientId)
        {
            var client = await _dbContext.Clients
                .Include(item => item.Tenant)
                .Include(item => item.User)
                .FirstOrDefaultAsync(item => item.Id == clientId);

            if (client is null)
            {
                return;
            }

            var disclosures =
                await _dbContext.Disclosures.Where(item => item.TenantId == client.TenantId).ToListAsync();

            var pdfData = await _pmuPdfService.GetPdfAsync(disclosures);

            var path = await _fileService.SaveAsync(BrowTricksCompany.Slug, GetFilePath(client), "application/pdf",
                new MemoryStream(pdfData));

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

        private List<string> GetFilePath(Client client)
        {
            return new List<string>
            {
                "pmu",
                client.Id.ToString()
            };
        }
    }
}