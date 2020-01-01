namespace WhyNotEarth.Meredith.Notifications.Email.Senders
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using SendGrid;
    using SendGrid.Helpers.Mail;

    public class SendGridEmailSender : IEmailSender
    {
        protected SendGridOptions SendGridOptions { get; }

        public SendGridEmailSender(
            IOptions<SendGridOptions> sendGridOptions
        )
        {
            SendGridOptions = sendGridOptions.Value;
        }

        public async Task SendEmail(EmailPayload emailPayload)
        {
            var client = new SendGridClient(new SendGridClientOptions { ApiKey = SendGridOptions.ApiKey });
            var message = MailHelper.CreateSingleTemplateEmail(
                new EmailAddress("noreply@whynot.earth", "Meredith"),
                await CreateToEmailAddress(emailPayload),
                emailPayload.TemplateId,
                emailPayload.TemplatePayload
            );
            await client.SendEmailAsync(message);
        }

        public virtual Task<EmailAddress> CreateToEmailAddress(EmailPayload emailPayload)
        {
            return Task.FromResult(new EmailAddress(emailPayload.RecipientAddress, emailPayload.RecipientName));
        }
    }
}