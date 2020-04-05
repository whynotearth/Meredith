using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        private readonly SendGridOptions _sendGridOptions;

        public SendGridService(IOptions<SendGridOptions> sendGridOptions, MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
            _sendGridOptions = sendGridOptions.Value;
        }

        public async Task SendEmail(string emailFrom, string emailFromName, string emailTo, string emailToName,
            string templateId, object templateData)
        {
            var client = new SendGridClient(_sendGridOptions.ApiKey);

            var from = new EmailAddress(emailFrom, emailFromName);
            var to = new EmailAddress(emailTo, emailToName);

            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, templateData);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode >= HttpStatusCode.Ambiguous)
            {
                var errorMessage = await GetErrorMessage(response);
                throw new Exception(errorMessage);
            }
        }

        public async Task SendEmail(int companyId, string emailTo, string emailToName, Dictionary<string, object> templateData)
        {
            var client = new SendGridClient(_sendGridOptions.ApiKey);

            var sendGridAccount = await GetAccount(companyId);
            var from = new EmailAddress(sendGridAccount.FromEmail, sendGridAccount.FromEmailName);
            var to = new EmailAddress(emailTo, emailToName);

            var msg = MailHelper.CreateSingleTemplateEmail(from, to, sendGridAccount.TemplateId, templateData);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode >= HttpStatusCode.Ambiguous)
            {
                var errorMessage = await GetErrorMessage(response);
                throw new Exception(errorMessage);
            }
        }

        private async Task<string> GetErrorMessage(Response response)
        {
            var body = await response.DeserializeResponseBodyAsync(response.Body);

            return string.Join(", ", body.Select(item => item.Key + ":" + item.Value).ToArray());
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
    }
}