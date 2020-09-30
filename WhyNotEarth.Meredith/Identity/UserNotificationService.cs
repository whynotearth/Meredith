using System;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Emails;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Twilio;

namespace WhyNotEarth.Meredith.Identity
{
    internal class UserNotificationService : IUserNotificationService
    {
        private readonly SendGridService _sendGridService;
        private readonly ITwilioService _twilioService;

        public UserNotificationService(ITwilioService twilioService, SendGridService sendGridService)
        {
            _twilioService = twilioService;
            _sendGridService = sendGridService;
        }

        public Task NotifyAsync(User user, Notification notification)
        {
            if (user.EmailConfirmed)
            {
                return NotifyAsync(user, NotificationType.Email, notification);
            }

            if (user.PhoneNumberConfirmed)
            {
                return NotifyAsync(user, NotificationType.Sms, notification);
            }

            return NotifyAsync(user, NotificationType.Email, notification);
        }

        public Task NotifyAsync(User user, NotificationType type, Notification notification)
        {
            if (type == NotificationType.Email)
            {
                return EmailAsync(notification.Company.Slug, user.Email, notification.Subject ?? string.Empty,
                    notification.Message);
            }

            if (type == NotificationType.Sms)
            {
                return SmsAsync(new ShortMessage
                {
                    CompanyId = notification.Company.Id,
                    To = user.PhoneNumber!,
                    Body = notification.Message,
                    CreatedAt = DateTime.UtcNow
                });
            }

            throw new NotSupportedException();
        }

        private Task EmailAsync(string companySlug, string email, string subject, string message)
        {
            return _sendGridService.SendAuthEmail(companySlug, email, subject, message);
        }

        private Task SmsAsync(ShortMessage shortMessage)
        {
            return _twilioService.SendAsync(shortMessage);
        }
    }
}