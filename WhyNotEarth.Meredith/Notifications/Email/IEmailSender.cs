namespace WhyNotEarth.Meredith.Notifications.Email
{
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        Task SendEmail(EmailPayload emailPayload);
    }
}