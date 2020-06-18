using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.Jobs.Volkswagen
{
    public class JumpStartEmailJob
    {
        private readonly MeredithDbContext _dbContext;
        private readonly JumpStartEmailTemplateService _jumpStartEmailTemplateService;
        private readonly JumpStartPdfJob _jumpStartPdfJob;
        private readonly SendGridService _sendGridService;

        public JumpStartEmailJob(MeredithDbContext dbContext, SendGridService sendGridService,
            JumpStartEmailTemplateService jumpStartEmailTemplateService, JumpStartPdfJob jumpStartPdfJob)
        {
            _dbContext = dbContext;
            _sendGridService = sendGridService;
            _jumpStartEmailTemplateService = jumpStartEmailTemplateService;
            _jumpStartPdfJob = jumpStartPdfJob;
        }

        public async Task SendAsync(int jumpStartId)
        {
            var jumpStart = await _dbContext.JumpStarts
                .FirstOrDefaultAsync(item => item.Id == jumpStartId && item.Status == JumpStartStatus.Sending);

            var articles = await _dbContext.Articles
                .Include(item => item.Image)
                .Include(item => item.Category)
                .ThenInclude(item => item.Image)
                .Where(item => item.Date == jumpStart.DateTime.Date)
                .OrderBy(item => item.Order).ToListAsync();

            await SendEmailAsync(jumpStart, articles);

            jumpStart.Status = JumpStartStatus.Sent;
            _dbContext.JumpStarts.Update(jumpStart);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SendEmailAsync(JumpStart jumpStart, List<Article> articles)
        {
            var emails = await GetRecipientsAsync(jumpStart.Id);
            //var recipients = emails.Select(item => Tuple.Create(item.EmailAddress, (string?) null)).ToList();

            var emailInfo = await GetEmailInfoAsync(jumpStart, emails, articles);

            await _sendGridService.SendEmailAsync(emailInfo);

            foreach (var emailRecipient in emails)
            {
                emailRecipient.Status = EmailStatus.Sent;
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<EmailInfo> GetEmailInfoAsync(JumpStart jumpStart, List<Data.Entity.Models.Email> emails, List<Article> articles)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Name == VolkswagenCompany.Slug);

            var pdfUrl = await _jumpStartPdfJob.CreatePdfUrlAsync(jumpStart);
            var emailTemplate = _jumpStartEmailTemplateService.GetEmailHtml(jumpStart.DateTime.Date, articles, pdfUrl);

            return new EmailInfo(company.Id, emails)
            {
                Subject = $"Project Blue Delta - {jumpStart.DateTime:MMMM d, yyyy}",
                HtmlContent = emailTemplate,
                PlainTextContent = emailTemplate,
                SendAt = jumpStart.DateTime
            };
        }

        private Task<List<Data.Entity.Models.Email>> GetRecipientsAsync(int jumpStartId)
        {
            return _dbContext.Emails
                .Where(item => item.JumpStartId == jumpStartId && item.Status == EmailStatus.ReadyToSend)
                .ToListAsync();
        }
    }
}