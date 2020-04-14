using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class MemoService
    {
        private const string MemoTemplateId = "d-5bf1030c93e04aed850ca9890fcb0b81";
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;
        private readonly SendGridService _sendGridService;

        public MemoService(MeredithDbContext dbContext, IBackgroundJobClient backgroundJobClient,
            SendGridService sendGridService)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
            _sendGridService = sendGridService;
        }

        public async Task CreateAsync(string subject, string date, string to, string description)
        {
            var recipients = await GetRecipients();

            // SendGrid accepts a maximum recipients of 1000 per API call
            // https://sendgrid.com/docs/for-developers/sending-email/v3-mail-send-faq/#are-there-limits-on-how-often-i-can-send-email-and-how-many-recipients-i-can-send-to
            foreach (var batch in Split(recipients, 900))
            {
                _backgroundJobClient.Enqueue<MemoService>(service =>
                    service.SendTestEmailAsync(subject, date, to, description, batch));
            }
        }

        public async Task SendTestEmailAsync(string subject, string date, string to, string description,
            List<Recipient> batch)
        {
            var templateData = new Dictionary<string, object>
            {
                {"subject", subject},
                {"date", date},
                {"to", to},
                {"description", description}
            };

            await _sendGridService.SendEmail("communications@vw.com", batch, MemoTemplateId, templateData);
        }

        private async Task<List<Recipient>> GetRecipients()
        {
            return await _dbContext.Recipients.ToListAsync();
        }

        private IEnumerable<List<T>> Split<T>(List<T> list, int batchSize)
        {
            for (var i = 0; i < list.Count; i += batchSize)
            {
                yield return list.GetRange(i, Math.Min(batchSize, list.Count - i));
            }
        }
    }
}