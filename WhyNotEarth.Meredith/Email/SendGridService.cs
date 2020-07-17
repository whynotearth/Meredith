using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Email
{
    public class SendGridService
    {
        private readonly MeredithDbContext _dbContext;

        // SendGrid accepts a maximum recipients of 1000 per API call
        // https://sendgrid.com/docs/for-developers/sending-email/v3-mail-send-faq/#are-there-limits-on-how-often-i-can-send-email-and-how-many-recipients-i-can-send-to
        public static int BatchSize { get; } = 900;

        public SendGridService(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendEmailAsync(EmailInfo emailInfo)
        {
            var sendGridAccount = await GetAccount(emailInfo.CompanyId, emailInfo.TemplateKey);

            if (!string.IsNullOrEmpty(sendGridAccount.Bcc))
            {
                emailInfo.Recipients.Add(new EmailInfoItem(Tuple.Create<string, string?>(sendGridAccount.Bcc, null)));
            }

            var from = new EmailAddress(sendGridAccount.FromEmail, sendGridAccount.FromEmailName);

            foreach (var batch in emailInfo.Recipients.Batch(BatchSize))
            {
                var sendGridMessage = GetSendGridMessage(batch, from, sendGridAccount, emailInfo);

                await Send(sendGridAccount, sendGridMessage);
            }
        }

        public async Task SendAuthEmail(string companySlug, string email, string subject, string message)
        {
            var sendGridAccount = await GetAccount(companySlug);

            var sendGridMessage = new SendGridMessage
            {
                From = new EmailAddress(sendGridAccount.FromEmail, sendGridAccount.FromEmailName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            sendGridMessage.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            sendGridMessage.SetClickTracking(false, false);

            await Send(sendGridAccount, sendGridMessage);
        }

        private SendGridMessage GetSendGridMessage(List<EmailInfoItem> batch, EmailAddress from,
            SendGridAccount sendGridAccount, EmailInfo emailInfo)
        {
            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetFrom(from);

            if (emailInfo.TemplateData != null)
            {
                sendGridMessage.TemplateId = sendGridAccount.TemplateId;
            }
            else
            {
                sendGridMessage.SetGlobalSubject(emailInfo.Subject);

                if (!string.IsNullOrEmpty(emailInfo.PlainTextContent))
                {
                    sendGridMessage.AddContent(MimeType.Text, emailInfo.PlainTextContent);
                }

                if (!string.IsNullOrEmpty(emailInfo.HtmlContent))
                {
                    sendGridMessage.AddContent(MimeType.Html, emailInfo.HtmlContent);
                }
            }

            for (var personalizationIndex = 0; personalizationIndex < batch.Count; ++personalizationIndex)
            {
                var emailInfoItem = batch[personalizationIndex];

                var to = GetEmailAddress(emailInfoItem);
                sendGridMessage.AddTo(to, personalizationIndex);

                if (emailInfo.TemplateData != null)
                {
                    sendGridMessage.SetTemplateData(emailInfo.TemplateData, personalizationIndex);
                }

                if (emailInfoItem.Email != null)
                {
                    sendGridMessage.AddCustomArg("EmailId", emailInfoItem.Email.Id.ToString(), personalizationIndex);
                }
            }

            if (emailInfo.SendAt != null)
            {
                sendGridMessage.SendAt = new DateTimeOffset(emailInfo.SendAt.Value).ToUnixTimeSeconds();
            }

            if (emailInfo.AttachmentName != null)
            {
                sendGridMessage.AddAttachment(emailInfo.AttachmentName, emailInfo.AttachmentBase64Content);
            }

            return sendGridMessage;
        }

        private async Task Send(SendGridAccount sendGridAccount, SendGridMessage sendGridMessage)
        {
            var client = new SendGridClient(sendGridAccount.ApiKey);

            var response = await client.SendEmailAsync(sendGridMessage);

            if (response.StatusCode >= HttpStatusCode.Ambiguous)
            {
                var errorMessage = await GetErrorMessage(response);
                throw new Exception(errorMessage);
            }
        }

        private async Task<SendGridAccount> GetAccount(int companyId, string? key)
        {
            var query = _dbContext.SendGridAccounts
                .Where(s => s.CompanyId == companyId);

            if (key != null)
            {
                query = query.Where(item => item.Key == key);
            }

            var sendGridAccount = await query.FirstOrDefaultAsync();

            if (sendGridAccount is null)
            {
                if (await _dbContext.Companies.AnyAsync(c => c.Id == companyId))
                {
                    throw new RecordNotFoundException($"Company {companyId} does not have SendGrid configured");
                }

                throw new RecordNotFoundException($"Company {companyId} not found");
            }

            return sendGridAccount;
        }

        private async Task<SendGridAccount> GetAccount(string companySlug)
        {
            var company = await _dbContext.Companies
                .Where(s => s.Slug == companySlug.ToLower())
                .FirstOrDefaultAsync();

            return await GetAccount(company.Id, null);
        }

        private async Task<string> GetErrorMessage(Response response)
        {
            var body = await response.DeserializeResponseBodyAsync(response.Body);

            return string.Join(", ", body.Select(item => item.Key + ":" + item.Value).ToArray());
        }

        private EmailAddress GetEmailAddress(EmailInfoItem emailInfoItem)
        {
            return new EmailAddress(emailInfoItem.Info.Item1, emailInfoItem.Info.Item2);
        }
    }

    public class EmailInfo
    {
        public int CompanyId { get; }

        public string? TemplateKey { get; set; }

        public List<EmailInfoItem> Recipients { get; }

        public object? TemplateData { get; set; }

        public string? Subject { get; set; }

        public string? PlainTextContent { get; set; }

        public string? HtmlContent { get; set; }

        public DateTime? SendAt { get; set; }

        public string? AttachmentName { get; set; }

        public string? AttachmentBase64Content { get; set; }

        public EmailInfo(int companyId, Tuple<string, string?> email) : this(companyId,
            new List<Tuple<string, string?>> { email })
        {
        }

        public EmailInfo(int companyId, List<Tuple<string, string?>> emails)
        {
            CompanyId = companyId;
            Recipients = emails.Select(item => new EmailInfoItem(item)).ToList();
        }

        public EmailInfo(int companyId, List<Data.Entity.Models.Email> emails)
        {
            CompanyId = companyId;
            Recipients = emails.Select(item => new EmailInfoItem(item)).ToList();
        }
    }

    public class EmailInfoItem
    {
        public Tuple<string, string?> Info { get; }

        public Data.Entity.Models.Email? Email { get; }

        public EmailInfoItem(Tuple<string, string?> info)
        {
            Info = info;
        }

        public EmailInfoItem(Data.Entity.Models.Email email)
        {
            Info = new Tuple<string, string?>(email.EmailAddress, null);
            Email = email;
        }
    }
}