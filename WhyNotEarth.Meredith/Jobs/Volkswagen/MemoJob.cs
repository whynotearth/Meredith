using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.Jobs.Volkswagen
{
    public class MemoJob
    {
        private const string TemplateKey = "Memo";

        private readonly MeredithDbContext _dbContext;
        private readonly SendGridService _sendGridService;

        public MemoJob(MeredithDbContext dbContext, SendGridService sendGridService)
        {
            _dbContext = dbContext;
            _sendGridService = sendGridService;
        }

        public async Task SendAsync(int memoId)
        {
            var memo = await _dbContext.Memos.FirstOrDefaultAsync(item => item.Id == memoId);
            
            var recipients = await _dbContext.Emails
                .Where(item => item.MemoId == memoId && item.Status == EmailStatus.ReadyToSend)
                .ToListAsync();

            var emailInfo = await GetEmailInfoAsync(memo, recipients);
            
            await _sendGridService.SendEmailAsync(emailInfo);

            foreach (var recipient in recipients)
            {
                recipient.Status = EmailStatus.Sent;
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<EmailInfo> GetEmailInfoAsync(Memo memo, List<Data.Entity.Models.Email> emails)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Name == VolkswagenCompany.Slug);

            var result = new EmailInfo(company.Id, emails)
            {
                TemplateKey = TemplateKey,
                TemplateData = GetTemplateData(memo),
            };

            if (memo.PdfUrl != null)
            {
                var attachmentContent = await GetPdfContentAsync(memo.PdfUrl);

                result.AttachmentName = "attachment.pdf";
                result.AttachmentBase64Content = attachmentContent;
            }

            return result;
        }

        private Dictionary<string, object> GetTemplateData(Memo memo)
        {
            return new Dictionary<string, object>
            {
                {"subject", memo.Subject},
                {"date", memo.Date},
                {"to", memo.To},
                // To handle new lines correctly we are replacing them with <br> and use {{{description}}}
                // so we have to html encode here
                {"description", Regex.Replace(HttpUtility.HtmlEncode(memo.Description), @"\r\n?|\n", "<br>")}
            };
        }

        private async Task<string> GetPdfContentAsync(string pdfUrl)
        {
            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(pdfUrl);

            return Convert.ToBase64String(bytes);
        }
    }
}