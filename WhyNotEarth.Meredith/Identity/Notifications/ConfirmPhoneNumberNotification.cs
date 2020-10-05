using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity.Notifications
{
    internal class ConfirmPhoneNumberNotification : Notification
    {
        private readonly Company _company;
        private readonly Public.Tenant? _tenant;
        private readonly string _token;

        public ConfirmPhoneNumberNotification(Company company, Public.Tenant? tenant, string token) : base(company)
        {
            _company = company;
            _tenant = tenant;
            _token = token;
        }

        public override string GetMessage(NotificationType notificationType)
        {
            return $"Code: {_token}\r\n{_company.Name} {_tenant?.Name}";
        }
    }
}