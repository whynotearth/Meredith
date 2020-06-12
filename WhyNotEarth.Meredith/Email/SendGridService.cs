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
                emailInfo.Recipients.Add(Tuple.Create<string, string?>(sendGridAccount.Bcc, null));
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

        private SendGridMessage GetSendGridMessage(List<Tuple<string, string?>> batch, EmailAddress from,
            SendGridAccount sendGridAccount, EmailInfo emailInfo)
        {
            SendGridMessage sendGridMessage;

            if (emailInfo.TemplateData != null)
            {
                sendGridMessage = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(from,
                    GetEmailAddresses(batch), sendGridAccount.TemplateId, emailInfo.TemplateData);
            }
            else
            {
                sendGridMessage = MailHelper.CreateSingleEmailToMultipleRecipients(from, GetEmailAddresses(batch),
                    emailInfo.Subject, emailInfo.PlainTextContent, emailInfo.HtmlContent);
            }

            if (emailInfo.UniqueArgument != null)
            {
                foreach (var personalization in sendGridMessage.Personalizations)
                {
                    personalization.CustomArgs = new Dictionary<string, string>
                    {
                        {emailInfo.UniqueArgument, emailInfo.UniqueArgumentValue ?? string.Empty},
                        {nameof(SendGridAccount.CompanyId), emailInfo.CompanyId.ToString()}
                    };
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

        private List<EmailAddress> GetEmailAddresses(List<Tuple<string, string?>> recipients)
        {
            return recipients.Select(item => new EmailAddress(item.Item1, item.Item2)).ToList();
        }
    }

    public class EmailInfo
    {
        public int CompanyId { get; }

        public string TemplateKey { get; set; }

        public List<Tuple<string, string?>> Recipients { get; }

        public object? TemplateData { get; set; }

        public string? Subject { get; set; }

        public string? PlainTextContent { get; set; }

        public string? HtmlContent { get; set; }

        public string? UniqueArgument { get; set; }

        public string? UniqueArgumentValue { get; set; }

        public DateTime? SendAt { get; set; }

        public string? AttachmentName { get; set; }

        public string? AttachmentBase64Content { get; set; }

        public EmailInfo(int companyId, Tuple<string, string?> recipient)
        {
            CompanyId = companyId;
            Recipients = new List<Tuple<string, string?>> {recipient};
        }

        public EmailInfo(int companyId, List<Tuple<string, string?>> recipients)
        {
            CompanyId = companyId;
            Recipients = recipients;
        }
    }
}