using System;
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
            var tos = new List<Tuple<string, string?>>();
            var subjects = new List<string>();
            var substitutions = new List<Dictionary<string, string>>();

            var pdfUrl = await _jumpStartPdfService.CreatePdfUrlAsync(jumpStart);
            var recipients = await GetRecipients(jumpStart);

            foreach (var recipient in recipients)
            {
                tos.Add(new Tuple<string, string?>(recipient.Email, null));

                subjects.Add("Subject");

                substitutions.Add(new Dictionary<string, string>
                {
                    {"{{print_url}}", pdfUrl}
                });
            }

            var emailTemplate = _jumpStartEmailTemplateService.GetEmailHtml(jumpStart.DateTime, jumpStart.Posts);

            await _sendGridService.SendEmail(company.Id, tos, subjects, emailTemplate, emailTemplate, substitutions,
                jumpStart.DateTime);
        }

        private Task<List<Recipient>> GetRecipients(JumpStart jumpStart)
        {
            var distributionGroups = jumpStart.DistributionGroups.Split(',');

            return _dbContext.Recipients.Where(item => distributionGroups.Contains(item.DistributionGroup))
                .ToListAsync();
        }
    }
}