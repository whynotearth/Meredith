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
            await SendEmailCore(companyId, recipients,
                true, templateData,
                null, null, null, null,
                null, null);
        }

        public async Task SendEmail(int companyId, List<Tuple<string, string?>> recipients, object templateData,
            string uniqueArgument, string uniqueArgumentValue)
        {
            await SendEmailCore(companyId, recipients, 
                true, templateData,
                null, null, null, null,
                uniqueArgument, uniqueArgumentValue);
        }

        public async Task SendEmail(int companyId, List<Tuple<string, string?>> recipients, List<string> subjects,
            string plainTextContent, string htmlContent, List<Dictionary<string, string>> substitutions)
        {
            await SendEmailCore(companyId, recipients, 
                false, null, 
                subjects, plainTextContent, htmlContent, substitutions,
                null, null);
        }

        private async Task SendEmailCore(int companyId, List<Tuple<string, string?>> recipients,
            bool useTemplate, object? templateData,
            List<string>? subjects, string? plainTextContent, string? htmlContent, List<Dictionary<string, string>>? substitutions,
            string? uniqueArgument, string? uniqueArgumentValue)
        {
            var sendGridAccount = await GetAccount(companyId);

            var client = new SendGridClient(sendGridAccount.ApiKey);
            var from = new EmailAddress(sendGridAccount.FromEmail, sendGridAccount.FromEmailName);

            var recipientEmailAddresses = recipients.Select(item => new EmailAddress(item.Item1, item.Item2)).ToList();

            if (!string.IsNullOrEmpty(sendGridAccount.Bcc))
            {
                recipientEmailAddresses.Add(new EmailAddress(sendGridAccount.Bcc));
            }

            SendGridMessage sendGridMessage;

            if (useTemplate)
            {
                sendGridMessage = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(from,
                    recipientEmailAddresses, sendGridAccount.TemplateId, templateData);
            }
            else
            {
                sendGridMessage = MailHelper.CreateMultipleEmailsToMultipleRecipients(from, recipientEmailAddresses,
                    subjects, plainTextContent, htmlContent, substitutions);
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

        private async Task<string> GetErrorMessage(Response response)
        {
            var body = await response.DeserializeResponseBodyAsync(response.Body);

            return string.Join(", ", body.Select(item => item.Key + ":" + item.Value).ToArray());
        }
    }
}