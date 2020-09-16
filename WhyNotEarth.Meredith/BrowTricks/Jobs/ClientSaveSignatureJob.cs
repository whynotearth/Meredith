using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.HelloSign;
using WhyNotEarth.Meredith.Pdf;
using WhyNotEarth.Meredith.Services;
using WhyNotEarth.Meredith.Twilio;

namespace WhyNotEarth.Meredith.BrowTricks.Jobs
{
    internal class ClientSaveSignatureJob : IClientSaveSignatureJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IDbContext _dbContext;
        private readonly IFileService _fileService;
        private readonly IHtmlService _htmlService;
        private readonly FormNotifications _formNotifications;
        private readonly IFormSignatureFileService _formSignatureFileService;

        public ClientSaveSignatureJob(IDbContext dbContext, IFormSignatureFileService formSignatureFileService,
            FormNotifications formNotifications, IBackgroundJobClient backgroundJobClient, IFileService fileService,
            IHtmlService htmlService)
        {
            _dbContext = dbContext;
            _formSignatureFileService = formSignatureFileService;
            _formNotifications = formNotifications;
            _backgroundJobClient = backgroundJobClient;
            _fileService = fileService;
            _htmlService = htmlService;
        }

        public async Task SaveSignature(int formSignatureId)
        {
            var formSignature = await _dbContext.FormSignatures
                .Include(item => item.Client)
                .ThenInclude(item => item.User)
                .Include(item => item.Client)
                .ThenInclude(item => item.Tenant)
                .Include(item => item.Answers)
                .FirstOrDefaultAsync(item => item.Id == formSignatureId);

            if (formSignature is null)
            {
                throw new ArgumentException($"Invalid formSignatureId: {formSignatureId}", nameof(formSignatureId));
            }

            var html = _formSignatureFileService.GetHtml(formSignature);

            var pdfData = await _htmlService.ToPdfAsync(html);

            var path = await _fileService.SaveAsync(BrowTricksCompany.Slug, GetFilePath(formSignature),
                "application/pdf", new MemoryStream(pdfData));

            formSignature.Html = html;
            formSignature.PdfPath = path;
            _dbContext.FormSignatures.Update(formSignature);
            await _dbContext.SaveChangesAsync();

            await SendPmuCompletionNotificationAsync(formSignature);
        }

        private async Task SendPmuCompletionNotificationAsync(FormSignature formSignature)
        {
            if (string.IsNullOrWhiteSpace(formSignature.Client.User.PhoneNumber))
            {
                return;
            }

            var pdfUrl = _fileService.GetPrivateUrl(formSignature.PdfPath!);

            var shortMessage =
                _formNotifications.GetCompletionNotification(formSignature.Client.Tenant, formSignature.Client.User,
                    pdfUrl);

            _dbContext.ShortMessages.Add(shortMessage);
            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<ITwilioService>(service =>
                service.SendAsync(shortMessage.Id));
        }

        private List<string> GetFilePath(FormSignature formSignature)
        {
            return new List<string>
            {
                "signatures",
                formSignature.FormTemplateId.ToString(),
                formSignature.Id.ToString()
            };
        }
    }
}