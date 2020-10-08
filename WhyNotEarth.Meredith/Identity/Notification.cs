using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity
{
    public abstract class Notification
    {
        public Company Company { get; }

        protected Notification(Company company)
        {
            Company = company;
        }

        public abstract string GetMessage(NotificationType notificationType);

        public abstract string GetSubject();
    }
}