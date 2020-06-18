using System;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class EmailRecipientResult
    {
        public string Email { get; }

        public DateTime? DeliverDateTime { get; }

        public DateTime? OpenDateTime { get; }

        public EmailRecipientResult(Data.Entity.Models.Email email)
        {
            Email = email.EmailAddress;
            DeliverDateTime = email.Events.OrderBy(item => item.DateTime).FirstOrDefault(item => item.Type == EmailEventType.Delivered)?.DateTime;
            OpenDateTime = email.Events.OrderBy(item => item.DateTime).FirstOrDefault(item => item.Type == EmailEventType.Opened)?.DateTime;
        }
    }
}