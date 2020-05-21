using System;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class EmailRecipientResult
    {
        public string Email { get; }

        public DateTime? DeliverDateTime { get; }

        public DateTime? OpenDateTime { get; }

        public EmailRecipientResult(EmailRecipient emailRecipient)
        {
            Email = emailRecipient.Email;
            DeliverDateTime = emailRecipient.DeliverDateTime;
            OpenDateTime = emailRecipient.OpenDateTime;
        }
    }
}