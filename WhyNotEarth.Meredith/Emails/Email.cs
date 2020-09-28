using System;
using System.Collections.Generic;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.Emails
{
    public class Email
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public int? MemoId { get; set; }

        public Memo? Memo { get; set; }

        public int? JumpStartId { get; set; }

        public JumpStart? JumpStart { get; set; }

        public int? NewJumpStartId { get; set; }

        public NewJumpStart? NewJumpStart { get; set; }

        public string EmailAddress { get; set; } = null!;

        public string? Group { get; set; }

        public EmailStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<EmailEvent>? Events { get; set; }
    }

    public enum EmailStatus : byte
    {
        None = 0,
        ReadyToSend = 1,
        Sent = 2,
        Delivered = 3,
        Opened = 4,
        Clicked = 5
    }
}