namespace WhyNotEarth.Meredith.Notifications.Email
{
    public class EmailPayload
    {
        public string RecipientAddress { get; set; }

        public string RecipientName { get; set; }

        public string TemplateId { get; set; }

        public string TemplatePayload { get; set; }
    }
}