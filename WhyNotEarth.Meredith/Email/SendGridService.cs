using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WhyNotEarth.Meredith.Email
{
    public class SendGridService
    {
        private readonly SendGridOptions _sendGridOptions;

        public SendGridService(IOptions<SendGridOptions> sendGridOptions)
        {
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

        private async Task<string> GetErrorMessage(Response response)
        {
            var body = await response.DeserializeResponseBodyAsync(response.Body);

            return string.Join(", ", body.Select(item => item.Key + ":" + item.Value).ToArray());
        }
    }
}