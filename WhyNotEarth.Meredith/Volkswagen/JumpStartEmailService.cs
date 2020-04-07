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
            var jumpStart = await _dbContext.JumpStarts.Include(item => item.Posts)
                .FirstOrDefaultAsync(item => item.Id == jumpStartId && item.Status == JumpStartStatus.ReadyToSend);

            var posts = jumpStart.Posts.OrderBy(item => item.Order);

            await SendEmailAsync(jumpStart);

            jumpStart.Status = JumpStartStatus.Sent;
            _dbContext.JumpStarts.Update(jumpStart);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SendEmailAsync(JumpStart jumpStart)
        {
            var tos = new List<Tuple<string, string>>();
            var subjects = new List<string>();
            var substitutions = new List<Dictionary<string, string>>();
            foreach (var recipient in GetRecipients())
            {
                tos.Add(new Tuple<string, string>(recipient.Email, recipient.Name));

                subjects.Add("Subject");

                substitutions.Add(new Dictionary<string, string>
                {
                    {"{{print_url}}", await _jumpStartPdfService.CreatePdfUrlAsync(jumpStart)}
                });
            }

            var emailTemplate = _jumpStartEmailTemplateService.GetEmailTemplate();

            // TODO: Fix these variables
            await _sendGridService.SendEmail("info@example.com", "Example", tos, subjects, emailTemplate,
                emailTemplate, substitutions);
        }

        private List<Recipient> GetRecipients()
        {
            // TODO: Implement

            return new List<Recipient>
            {
                new Recipient("ShrGholami@gmail.com", "CT")
            };
        }

        private class Recipient
        {
            public string Email { get; }

            public string Name { get; }

            public Recipient(string email, string name)
            {
                Email = email;
                Name = name;
            }
        }
    }
}