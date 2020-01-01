namespace WhyNotEarth.Meredith.Notifications.Email
{
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Data.Entity.Models;
    using WhyNotEarth.Meredith.Jobs;

    public class EmailService : IEmailService
    {
        protected MeredithDbContext MeredithDbContext { get; }

        protected IJob Job { get; }

        public EmailService(
            MeredithDbContext meredithDbContext,
            IJob job)
        {
            MeredithDbContext = meredithDbContext;
            Job = job;
        }

        public Task SendConfirmationEmail(User user, string token) => SendUserTokenEmail(Templates.ConfirmEmail, user, token);
        public Task SendPasswordResetEmail(User user, string token) => SendUserTokenEmail(Templates.PasswordReset, user, token);

        protected async Task SendUserTokenEmail(string templateId, User user, string token)
        {
            await QueueEmail(user, templateId, new
            {
                user = new
                {
                    user.Id,
                    user.UserName
                },
                token = System.Web.HttpUtility.UrlEncode(token)
            });
        }

        protected async Task QueueEmail(User user, string templateId, object data)
        {
            var contractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            };
            var email = new EmailPayload
            {
                RecipientAddress = user.Email,
                RecipientName = user.UserName,
                TemplateId = templateId,
                TemplatePayload = JsonConvert.SerializeObject(data, serializerSettings)
            };
            await Job.Enqueue<IEmailSender>(es => es.SendEmail(email));
        }
    }
}