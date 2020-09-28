using System;

namespace WhyNotEarth.Meredith.Emails
{
    public class EmailEvent
    {
        public int Id { get; set; }

        public int EmailId { get; set; }

        public Email Email { get; set; } = null!;

        public EmailEventType Type { get; set; }

        public DateTime DateTime { get; set; }
    }

    public enum EmailEventType : byte
    {
        None = 0,
        Delivered = 1,
        Opened = 2,
        Clicked = 3
    }
}