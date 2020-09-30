using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Emails
{
    public class SendGridService
    {
        private readonly IDbContext _dbContext;

        // SendGrid accepts a maximum recipients of 1000 per API call
        // https://sendgrid.com/docs/for-developers/sending-email/v3-mail-send-faq/#are-there-limits-on-how-often-i-can-send-email-and-how-many-recipients-i-can-send-to
        public static int BatchSize { get; } = 900;

        public SendGridService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            var sendGridAccount = await GetAccount(emailMessage.CompanyId, emailMessage.TemplateKey);

            if (!string.IsNullOrEmpty(sendGridAccount.Bcc))
            {
                emailMessage.Recipients.Add(new EmailMessageRecipient(Tuple.Create<string, string?>(sendGridAccount.Bcc, null)));
            }

            var from = new EmailAddress(sendGridAccount.FromEmail, sendGridAccount.FromEmailName);

            foreach (var batch in emailMessage.Recipients.Batch(BatchSize))
            {
                var sendGridMessage = GetSendGridMessage(batch, from, sendGridAccount, emailMessage);

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

        private SendGridMessage GetSendGridMessage(List<EmailMessageRecipient> batch, EmailAddress from,
            SendGridAccount sendGridAccount, EmailMessage emailMessage)
        {
            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetFrom(from);

            if (emailMessage.TemplateData != null)
            {
                sendGridMessage.TemplateId = sendGridAccount.TemplateId;
            }
            else
            {
                sendGridMessage.SetGlobalSubject(emailMessage.Subject);

                if (!string.IsNullOrEmpty(emailMessage.PlainTextContent))
                {
                    sendGridMessage.AddContent(MimeType.Text, emailMessage.PlainTextContent);
                }

                if (!string.IsNullOrEmpty(emailMessage.HtmlContent))
                {
                    sendGridMessage.AddContent(MimeType.Html, emailMessage.HtmlContent);
                }
            }

            for (var personalizationIndex = 0; personalizationIndex < batch.Count; ++personalizationIndex)
            {
                var emailInfoItem = batch[personalizationIndex];

                var to = GetEmailAddress(emailInfoItem);
                sendGridMessage.AddTo(to, personalizationIndex);

                if (emailMessage.TemplateData != null)
                {
                    sendGridMessage.SetTemplateData(emailMessage.TemplateData, personalizationIndex);
                }

                if (emailInfoItem.EmailId != null)
                {
                    sendGridMessage.AddCustomArg("EmailId", emailInfoItem.EmailId.ToString(), personalizationIndex);
                }
            }

            if (emailMessage.SendAt != null)
            {
                sendGridMessage.SendAt = new DateTimeOffset(emailMessage.SendAt.Value).ToUnixTimeSeconds();
            }

            if (emailMessage.AttachmentName != null)
            {
                sendGridMessage.AddAttachment(emailMessage.AttachmentName, emailMessage.AttachmentBase64Content);
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

        private EmailAddress GetEmailAddress(EmailMessageRecipient emailMessageRecipient)
        {
            return new EmailAddress(emailMessageRecipient.Info.Item1, emailMessageRecipient.Info.Item2);
        }
    }

    public class EmailMessage
    {
        public int CompanyId { get; }

        public string? TemplateKey { get; set; }

        public List<EmailMessageRecipient> Recipients { get; }

        public object? TemplateData { get; set; }

        public string? Subject { get; set; }

        public string? PlainTextContent { get; set; }

        public string? HtmlContent { get; set; }

        public DateTime? SendAt { get; set; }

        public string? AttachmentName { get; set; }

        public string? AttachmentBase64Content { get; set; }

        public EmailMessage(int companyId, Tuple<string, string?> email) : this(companyId,
            new List<Tuple<string, string?>> { email })
        {
        }

        public EmailMessage(int companyId, List<Tuple<string, string?>> emails)
        {
            CompanyId = companyId;
            Recipients = emails.Select(item => new EmailMessageRecipient(item)).ToList();
        }

        public EmailMessage(int companyId, List<Email> emails)
        {
            CompanyId = companyId;
            Recipients = emails.Select(item => new EmailMessageRecipient(item)).ToList();
        }
    }

    public class EmailMessageRecipient
    {
        public Tuple<string, string?> Info { get; }

        public int? EmailId { get; }

        public EmailMessageRecipient(Tuple<string, string?> info)
        {
            Info = info;
        }

        public EmailMessageRecipient(Email email)
        {
            Info = new Tuple<string, string?>(email.EmailAddress, null);
            EmailId = email.Id;
        }
    }
}