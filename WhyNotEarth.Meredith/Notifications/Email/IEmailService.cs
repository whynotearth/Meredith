namespace WhyNotEarth.Meredith.Notifications.Email
{
    using System.Threading.Tasks;
    using WhyNotEarth.Meredith.Data.Entity.Models;

    public interface IEmailService
    {
        Task SendConfirmationEmail(User user, string token);
        Task SendPasswordResetEmail(User user, string token);
    }
}