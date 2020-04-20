using System.Collections.Generic;
using System.Linq;
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

        public async Task CreateAsync(string distributionGroup, string subject, string date, string to,
            string description)
        {
            var memo = new Memo
            {
                DistributionGroup = distributionGroup,
                Subject = subject,
                Date = date,
                To = to,
                Description = description
            };

            _dbContext.Memos.Add(memo);
            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<MemoService>(service =>
                service.CreateMemoRecipients(memo.Id));
        }

        public async Task CreateMemoRecipients(int memoId)
        {
            var memo = await _dbContext.Memos.FirstOrDefaultAsync(item => item.Id == memoId);
            var recipients = await GetRecipients(memo.DistributionGroup);

            // In case something went wrong and this is a retry
            var oldMemoRecipients = await _dbContext.MemoRecipients.Where(item => item.MemoId == memoId).ToListAsync();
            _dbContext.MemoRecipients.RemoveRange(oldMemoRecipients);
            await _dbContext.SaveChangesAsync();

            foreach (var batch in recipients.Batch(100))
            {
                var memoRecipients = batch.Select(item => new MemoRecipient
                {
                    MemoId = memoId,
                    Email = item.Email,
                    Status = MemoStatus.ReadyToSend
                });

                _dbContext.MemoRecipients.AddRange(memoRecipients);
                await _dbContext.SaveChangesAsync();
            }

            _backgroundJobClient.Enqueue<MemoService>(service =>
                service.SendEmailAsync(memo.Id));
        }

        public async Task SendEmailAsync(int memoId)
        {
            var memo = await _dbContext.Memos.FirstOrDefaultAsync(item => item.Id == memoId);

            var templateData = new Dictionary<string, object>
            {
                {"subject", memo.Subject},
                {"date", memo.Date},
                {"to", memo.To},
                {"description", memo.Description}
            };

            var memoRecipients = await _dbContext.MemoRecipients
                .Where(item => item.MemoId == memoId && item.Status == MemoStatus.ReadyToSend)
                .ToListAsync();

            // SendGrid accepts a maximum recipients of 1000 per API call
            // https://sendgrid.com/docs/for-developers/sending-email/v3-mail-send-faq/#are-there-limits-on-how-often-i-can-send-email-and-how-many-recipients-i-can-send-to
            foreach (var batch in memoRecipients.Batch(900))
            {
                var recipients = batch.ToList();

                await _sendGridService.SendEmail("communications@vw.com", recipients, MemoTemplateId, templateData,
                    nameof(MemoRecipient.MemoId), memo.Id.ToString());

                foreach (var memoRecipient in recipients)
                {
                    memoRecipient.Status = MemoStatus.Sent;
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<List<Recipient>> GetRecipients(string distributionGroup)
        {
            return await _dbContext.Recipients
                .Where(item => item.DistributionGroup.ToLower() == distributionGroup.ToLower()).ToListAsync();
        }

        public Task<List<MemoRecipient>> GetStatsAsync()
        {
            return _dbContext.MemoRecipients.ToListAsync();
        }
    }
}