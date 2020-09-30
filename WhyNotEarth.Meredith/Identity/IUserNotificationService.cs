using System.Threading.Tasks;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity
{
    public interface IUserNotificationService
    {
        Task NotifyAsync(User user, Notification notification);

        Task NotifyAsync(User user, NotificationType type, Notification notification);
    }

    public enum NotificationType
    {
        Sms,
        Email
    }
}