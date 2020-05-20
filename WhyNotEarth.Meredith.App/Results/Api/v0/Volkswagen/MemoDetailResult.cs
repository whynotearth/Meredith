using System;
using System.Collections.Generic;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class MemoDetailResult
    {
        public MemoListResult MemoList { get; }

        public List<EmailRecipientResult> NotOpened { get; } = new List<EmailRecipientResult>();

        public List<EmailRecipientResult> Opened { get; } = new List<EmailRecipientResult>();

        public MemoDetailResult(MemoInfo memoInfo)
        {
            MemoList = new MemoListResult(memoInfo);
        }
    }

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