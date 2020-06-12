using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Services;

namespace WhyNotEarth.Meredith.Volkswagen.Jobs
{
    public class NewJumpStartEmailJob
    {
        private readonly MeredithDbContext _dbContext;
        private readonly IFileService _fileService;
        private readonly NewJumpStartService _newJumpStartService;
        private readonly SendGridService _sendGridService;

        public NewJumpStartEmailJob(MeredithDbContext dbContext, SendGridService sendGridService,
            NewJumpStartService newJumpStartService, IFileService fileService)
        {
            _dbContext = dbContext;
            _sendGridService = sendGridService;
            _newJumpStartService = newJumpStartService;
            _fileService = fileService;
        }

        public async Task SendAsync(int newJmpStartId)
        {
            var newJumpStart = await _dbContext.NewJumpStarts
                .FirstOrDefaultAsync(item => item.Id == newJmpStartId && item.Status == NewJumpStartStatus.Sending);

            await SendEmailAsync(newJumpStart);

            newJumpStart.Status = NewJumpStartStatus.Sent;
            _dbContext.NewJumpStarts.Update(newJumpStart);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SendEmailAsync(NewJumpStart newJumpStart)
        {
            var emailRecipients = await GetRecipientsAsync(newJumpStart.Id);
            var recipients = emailRecipients.Select(item => Tuple.Create(item.Email, (string?) null)).ToList();

            var emailInfo = await GetEmailInfoAsync(newJumpStart, recipients);

            await _sendGridService.SendEmailAsync(emailInfo);

            foreach (var emailRecipient in emailRecipients)
            {
                emailRecipient.Status = EmailStatus.Sent;
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<EmailInfo> GetEmailInfoAsync(NewJumpStart newJumpStart,
            List<Tuple<string, string?>> recipients)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Name == VolkswagenCompany.Slug);
            var attachmentContent = await GetPdfContentAsync(newJumpStart.DateTime);

            return new EmailInfo(company.Id, recipients)
            {
                TemplateKey = "NewJumpStart",
                TemplateData = GetTemplateData(newJumpStart),
                AttachmentName = $"{newJumpStart.DateTime.Date:yyyy_MM_dd}.pdf",
                AttachmentBase64Content = attachmentContent
            };
        }

        private Dictionary<string, object> GetTemplateData(NewJumpStart newJumpStart)
        {
            return new Dictionary<string, object>
            {
                {"subject", $"Project Blue Delta - {newJumpStart.DateTime:MMMM d, yyyy}"},
                {"date", newJumpStart.DateTime.ToString("dddd | MMM. d, yyyy")},
                {"to", newJumpStart.Status},
                {"description", newJumpStart.Body}
            };
        }

        private async Task<string> GetPdfContentAsync(DateTime dateTime)
        {
            var path = _newJumpStartService.GetPdfPath(dateTime);

            await using var stream = new MemoryStream();

            await _fileService.GetAsync(path, stream);
            var bytes = stream.ToArray();

            return Convert.ToBase64String(bytes);
        }

        private Task<List<EmailRecipient>> GetRecipientsAsync(int newJumpStartId)
        {
            return _dbContext.EmailRecipients
                .Where(item => item.NewJumpStartId == newJumpStartId && item.Status == EmailStatus.ReadyToSend)
                .ToListAsync();
        }
    }
}