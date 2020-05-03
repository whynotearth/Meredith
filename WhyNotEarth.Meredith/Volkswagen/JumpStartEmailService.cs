using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartEmailService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly JumpStartEmailTemplateService _jumpStartEmailTemplateService;
        private readonly JumpStartPdfService _jumpStartPdfService;
        private readonly SendGridService _sendGridService;

        public JumpStartEmailService(MeredithDbContext dbContext, SendGridService sendGridService,
            JumpStartEmailTemplateService jumpStartEmailTemplateService, JumpStartPdfService jumpStartPdfService)
        {
            _dbContext = dbContext;
            _sendGridService = sendGridService;
            _jumpStartEmailTemplateService = jumpStartEmailTemplateService;
            _jumpStartPdfService = jumpStartPdfService;
        }

        public async Task SendAsync(int jumpStartId)
        {
            var jumpStart = await _dbContext.JumpStarts
                .Include(item => item.Posts)
                .ThenInclude(item => item.Image)
                .Include(item => item.Posts)
                .ThenInclude(item => item.Category)
                .ThenInclude(item => item.Image)
                .FirstOrDefaultAsync(item => item.Id == jumpStartId && item.Status == JumpStartStatus.ReadyToSend);

            jumpStart.Posts = jumpStart.Posts.OrderBy(item => item.Order).ToList();

            await SendEmailAsync(jumpStart);

            jumpStart.Status = JumpStartStatus.Sent;
            _dbContext.JumpStarts.Update(jumpStart);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SendEmailAsync(JumpStart jumpStart)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Name == VolkswagenCompany.Name);

            var pdfUrl = await _jumpStartPdfService.CreatePdfUrlAsync(jumpStart);
            var emailTemplate = _jumpStartEmailTemplateService.GetEmailHtml(jumpStart.DateTime, jumpStart.Posts);

            var recipients = await GetRecipients(jumpStart.Id);

            foreach (var batch in recipients.Batch(SendGridService.BatchSize))
            {
                var subjects = Enumerable.Repeat("Subject", batch.Count).ToList();

                var substitutions = Enumerable.Repeat(new Dictionary<string, string>
                {
                    {"{{print_url}}", pdfUrl}
                }, batch.Count).ToList();

                await _sendGridService.SendEmail(company.Id, batch, subjects, emailTemplate, emailTemplate, substitutions,
                    jumpStart.DateTime);

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