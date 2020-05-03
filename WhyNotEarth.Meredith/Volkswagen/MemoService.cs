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
        private readonly MemoRecipientService _memoRecipientService;
        private readonly SendGridService _sendGridService;

        public MemoService(MeredithDbContext dbContext, IBackgroundJobClient backgroundJobClient,
            SendGridService sendGridService, MemoRecipientService memoRecipientService)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
            _sendGridService = sendGridService;
            _memoRecipientService = memoRecipientService;
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

            _backgroundJobClient.Enqueue<MemoRecipientService>(service =>
                service.CreateMemoRecipients(memo.Id));
        }

        public async Task<List<MemoInfo>> GetListAsync()
        {
            var memos = await _dbContext.Memos.OrderByDescending(item => item.CreationDateTime).ToListAsync();

            var result = new List<MemoInfo>();
            foreach (var memo in memos)
            {
                var memoStat = await _memoRecipientService.GetMemoListStats(memo.Id);

                result.Add(new MemoInfo(memo, memoStat));
            }

            return result;
        }

        public async Task<MemoInfo> Get(int memoId)
        {
            var memo = await _dbContext.Memos.FirstOrDefaultAsync(item => item.Id == memoId);

            var memoStat = await _memoRecipientService.GetMemoListStats(memo.Id);

            return new MemoInfo(memo, memoStat);;
        }

        public async Task SendEmailAsync(int memoId)
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

            var memoRecipients = await _dbContext.MemoRecipients
                .Where(item => item.MemoId == memoId && item.Status == MemoStatus.ReadyToSend)
                .ToListAsync();

            // SendGrid accepts a maximum recipients of 1000 per API call
            // https://sendgrid.com/docs/for-developers/sending-email/v3-mail-send-faq/#are-there-limits-on-how-often-i-can-send-email-and-how-many-recipients-i-can-send-to
            foreach (var batch in memoRecipients.Batch(900))
            {
                var cachedList = batch.ToList();
                var recipients = cachedList.Select(item => Tuple.Create(item.Email, string.Empty)).ToList();

                await _sendGridService.SendEmail(company.Id, recipients, templateData, nameof(MemoRecipient.MemoId), memo.Id.ToString());

                foreach (var memoRecipient in cachedList)
                {
                    memoRecipient.Status = MemoStatus.Sent;
                }

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}