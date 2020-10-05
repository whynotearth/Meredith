using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity
{
    public abstract class Notification
    {
        public Company Company { get; }

        public string? Subject { get; set; }

        protected Notification(Company company)
        {
            Company = company;
        }

        public abstract string GetMessage(NotificationType notificationType);
    }
}