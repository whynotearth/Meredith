using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Volkswagen.Jobs
{
    public class NewJumpStartEmailJob
    {
        private const string TemplateKey = "NewJumpStart";

        private readonly MeredithDbContext _dbContext;
        private readonly SendGridService _sendGridService;

        public NewJumpStartEmailJob(MeredithDbContext dbContext, SendGridService sendGridService)
        {
            _dbContext = dbContext;
            _sendGridService = sendGridService;
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

            var emailInfo = await GetEmailInfoAsync(newJumpStart, emailRecipients);

            await _sendGridService.SendEmailAsync(emailInfo);

            foreach (var emailRecipient in emailRecipients)
            {
                emailRecipient.Status = EmailStatus.Sent;
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<EmailMessage> GetEmailInfoAsync(NewJumpStart newJumpStart, List<Public.Email> emails)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Name == VolkswagenCompany.Slug);

            var result = new EmailMessage(company.Id, emails)
            {
                TemplateKey = TemplateKey,
                TemplateData = GetTemplateData(newJumpStart),
            };

            if (newJumpStart.PdfUrl != null)
            {
                var attachmentContent = await GetPdfContentAsync(newJumpStart.PdfUrl);

                result.AttachmentName = $"{newJumpStart.DateTime.Date:yyyy_MM_dd}.pdf";
                result.AttachmentBase64Content = attachmentContent;
            }

            return result;
        }

        private Dictionary<string, object> GetTemplateData(NewJumpStart newJumpStart)
        {
            return new Dictionary<string, object>
            {
                {"subject", $"Project Blue Delta - {newJumpStart.DateTime:MMMM d, yyyy}"},
                {"date", newJumpStart.DateTime.ToString("dddd | MMM. d, yyyy")},
                //{"to", newJumpStart.Status},
                {"description", newJumpStart.Body ?? string.Empty}
            };
        }

        private async Task<string> GetPdfContentAsync(string pdfUrl)
        {
            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(pdfUrl);

            return Convert.ToBase64String(bytes);
        }

        private Task<List<Public.Email>> GetRecipientsAsync(int newJumpStartId)
        {
            return _dbContext.Emails
                .Where(item => item.NewJumpStartId == newJumpStartId && item.Status == EmailStatus.ReadyToSend)
                .ToListAsync();
        }
    }
}