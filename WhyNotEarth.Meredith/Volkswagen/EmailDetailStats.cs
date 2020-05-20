using System.Collections.Generic;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class EmailDetailStats
    {
        public List<EmailRecipient> NotOpenedList { get; }

        public List<EmailRecipient> OpenedList { get; }

        public EmailDetailStats(List<EmailRecipient> notOpenedList, List<EmailRecipient> openedList)
        {
            NotOpenedList = notOpenedList;
            OpenedList = openedList;
        }
    }
}