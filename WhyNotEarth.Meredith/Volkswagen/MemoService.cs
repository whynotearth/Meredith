using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class MemoService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;
        private readonly EmailRecipientService _emailRecipientService;
        private readonly SendGridService _sendGridService;

        public MemoService(MeredithDbContext dbContext, IBackgroundJobClient backgroundJobClient,
            SendGridService sendGridService, EmailRecipientService emailRecipientService)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
            _sendGridService = sendGridService;
            _emailRecipientService = emailRecipientService;
        }

        public async Task CreateAsync(List<string> distributionGroups, string subject, string date, string to,
            string description)
        {
            var memo = new Memo
            {
                DistributionGroups = string.Join(',', distributionGroups),
                Subject = subject,
                Date = date,
                To = to,
                Description = description,
                CreationDateTime = DateTime.UtcNow
            };

            _dbContext.Memos.Add(memo);
            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<EmailRecipientService>(service =>
                service.CreateForMemo(memo.Id));
        }

        public async Task<List<MemoInfo>> GetListAsync()
        {
            var memos = await _dbContext.Memos.OrderByDescending(item => item.CreationDateTime).ToListAsync();

            var result = new List<MemoInfo>();
            foreach (var memo in memos)
            {
                var memoStat = await _emailRecipientService.GetMemoListStats(memo.Id);

                result.Add(new MemoInfo(memo, memoStat));
            }

            return result;
        }

        public async Task<MemoInfo> Get(int memoId)
        {
            var memo = await _dbContext.Memos.FirstOrDefaultAsync(item => item.Id == memoId);

            var memoStat = await _emailRecipientService.GetMemoListStats(memo.Id);

            return new MemoInfo(memo, memoStat);
            ;
        }

        public async Task SendAsync(int memoId)
        {
            var memo = await _dbContext.Memos.FirstOrDefaultAsync(item => item.Id == memoId);
            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Name == VolkswagenCompany.Name);

            var templateData = new Dictionary<string, object>
            {
                {"subject", memo.Subject},
                {"date", memo.Date},
                {"to", memo.To},
                // To handle new lines correctly we are replacing them with <br> and use {{{description}}}
                // so we have to html encode here
                {"description", Regex.Replace(HttpUtility.HtmlEncode(memo.Description), @"\r\n?|\n", "<br>")}
            };

            var recipients = await _dbContext.EmailRecipients
                .Where(item => item.MemoId == memoId && item.Status == EmailStatus.ReadyToSend)
                .ToListAsync();

            foreach (var batch in recipients.Batch(SendGridService.BatchSize))
            {
                await _sendGridService.SendEmail(company.Id, recipients, templateData, nameof(EmailRecipient.MemoId),
                    memo.Id.ToString());

                foreach (var recipient in batch)
                {
                    recipient.Status = EmailStatus.Sent;
                }

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}