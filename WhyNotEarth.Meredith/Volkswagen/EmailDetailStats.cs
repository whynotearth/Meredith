using System.Collections.Generic;
using WhyNotEarth.Meredith.Emails;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class EmailDetailStats
    {
        public List<Email> NotOpenedList { get; }

        public List<Email> OpenedList { get; }

        public EmailDetailStats(List<Email> notOpenedList, List<Email> openedList)
        {
            NotOpenedList = notOpenedList;
            OpenedList = openedList;
        }
    }
}