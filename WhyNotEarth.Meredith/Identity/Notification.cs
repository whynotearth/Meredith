using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity
{
    public class Notification
    {
        public Company Company { get; }

        public string? Subject { get; set; }

        public string Message { get; }

        public Notification(Company company, string message)
        {
            Company = company;
            Message = message;
        }
    }
}