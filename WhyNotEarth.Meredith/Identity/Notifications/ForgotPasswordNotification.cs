using System.Text.Encodings.Web;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity.Notifications
{
    internal class ForgotPasswordNotification : Notification
    {
        private readonly string _callbackUrl;

        public ForgotPasswordNotification(Company company, string callbackUrl) : base(company)
        {
            _callbackUrl = callbackUrl;
        }

        public override string GetMessage(NotificationType notificationType)
        {
            if (notificationType == NotificationType.Email)
            {
                return
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(_callbackUrl)}'>clicking here</a>.";
            }

            return $"Please reset your password by clicking here: {_callbackUrl}";
        }
    }
}