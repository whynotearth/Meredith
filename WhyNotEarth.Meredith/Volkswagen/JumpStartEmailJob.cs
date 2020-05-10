using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;

namespace WhyNotEarth.Meredith.Volkswagen
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
            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Name == VolkswagenCompany.Name);

            var pdfUrl = await _jumpStartPdfJob.CreatePdfUrlAsync(jumpStart);
            var emailTemplate = _jumpStartEmailTemplateService.GetEmailHtml(jumpStart.DateTime.Date, articles, pdfUrl);

            var recipients = await GetRecipients(jumpStart.Id);
            var subject =
                $"Project Blue Delta - {jumpStart.DateTime.InZone(VolkswagenCompany.TimeZoneId, "MMMM d, yyyy")}";

            foreach (var batch in recipients.Batch(SendGridService.BatchSize))
            {
                await _sendGridService.SendEmail(company.Id, batch, subject, emailTemplate, emailTemplate, jumpStart.DateTime);

                foreach (var recipient in batch)
                {
                    recipient.Status = EmailStatus.Sent;
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        private Task<List<EmailRecipient>> GetRecipients(int jumpStartId)
        {
            return _dbContext.EmailRecipients
                .Where(item => item.JumpStartId == jumpStartId && item.Status == EmailStatus.ReadyToSend)
                .ToListAsync();
        }
    }
}