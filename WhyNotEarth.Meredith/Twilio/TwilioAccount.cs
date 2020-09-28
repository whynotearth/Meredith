using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Twilio
{
    public class TwilioAccount
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public string AccountSid { get; set; } = null!;

        public string AuthToken { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;
    }
}