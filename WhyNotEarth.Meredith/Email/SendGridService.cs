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
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
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

        public async Task SendEmail(int companyId, Tuple<string, string?> recipient, object templateData)
        {
            await SendEmail(companyId, new List<Tuple<string, string?>> {recipient}, templateData);
        }

        public async Task SendEmail(int companyId, List<Tuple<string, string?>> recipients, object templateData)
        {
            var emailAddresses = recipients.Select(item => new EmailAddress(item.Item1, item.Item2)).ToList();

            await SendEmailCore(companyId, emailAddresses,
                true, templateData,
                null, null, null, null,
                null, null,
                null);
        }

        public async Task SendEmail(int companyId, List<EmailRecipient> recipients, object templateData,
            string uniqueArgument, string uniqueArgumentValue)
        {
            var emailAddresses = recipients.Select(item => new EmailAddress(item.Email)).ToList();

            await SendEmailCore(companyId, emailAddresses,
                true, templateData,
                null, null, null, null,
                uniqueArgument, uniqueArgumentValue,
                null);
        }

        public async Task SendEmail(int companyId, List<EmailRecipient> recipients, List<string> subjects,
            string plainTextContent, string htmlContent, List<Dictionary<string, string>> substitutions,
            DateTime sendAt)
        {
            var emailAddresses = recipients.Select(item => new EmailAddress(item.Email)).ToList();

            await SendEmailCore(companyId, emailAddresses,
                false, null,
                subjects, plainTextContent, htmlContent, substitutions,
                null, null,
                sendAt);
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

        private async Task SendEmailCore(int companyId, List<EmailAddress> recipients,
            bool useTemplate, object? templateData,
            List<string>? subjects, string? plainTextContent, string? htmlContent,
            List<Dictionary<string, string>>? substitutions,
            string? uniqueArgument, string? uniqueArgumentValue,
            DateTime? sendAt)
        {
            var sendGridAccount = await GetAccount(companyId);

            var from = new EmailAddress(sendGridAccount.FromEmail, sendGridAccount.FromEmailName);

            if (!string.IsNullOrEmpty(sendGridAccount.Bcc))
            {
                recipients.Add(new EmailAddress(sendGridAccount.Bcc));
                subjects?.Add(subjects.Last());
                substitutions?.Add(substitutions.Last());
            }

            SendGridMessage sendGridMessage;

            if (useTemplate)
            {
                sendGridMessage =
                    MailHelper.CreateSingleTemplateEmailToMultipleRecipients(from, recipients,
                        sendGridAccount.TemplateId, templateData);
            }
            else
            {
                sendGridMessage = MailHelper.CreateMultipleEmailsToMultipleRecipients(from, recipients, subjects,
                    plainTextContent, htmlContent, substitutions);
            }

            if (uniqueArgument != null && uniqueArgumentValue != null)
            {
                foreach (var personalization in sendGridMessage.Personalizations)
                {
                    personalization.CustomArgs = new Dictionary<string, string>
                    {
                        {uniqueArgument, uniqueArgumentValue}
                    };
                }
            }

            if (sendAt != null)
            {
                sendGridMessage.SendAt = new DateTimeOffset(sendAt.Value).ToUnixTimeSeconds();
            }

            await Send(sendGridAccount, sendGridMessage);
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

        private async Task<SendGridAccount> GetAccount(int companyId)
        {
            var sendGridAccount = await _dbContext.SendGridAccounts
                .Where(s => s.CompanyId == companyId)
                .FirstOrDefaultAsync();

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
            var sendGridAccount = await _dbContext.SendGridAccounts
                .Include(item => item.Company)
                .Where(s => s.Company.Slug.ToLower() == companySlug.ToLower())
                .FirstOrDefaultAsync();

            if (sendGridAccount is null)
            {
                if (await _dbContext.Companies.AnyAsync(c => c.Slug.ToLower() == companySlug.ToLower()))
                {
                    throw new RecordNotFoundException($"Company {companySlug} does not have SendGrid configured");
                }

                throw new RecordNotFoundException($"Company {companySlug} not found");
            }

            return sendGridAccount;
        }

        private async Task<string> GetErrorMessage(Response response)
        {
            var body = await response.DeserializeResponseBodyAsync(response.Body);

            return string.Join(", ", body.Select(item => item.Key + ":" + item.Value).ToArray());
        }
    }
}