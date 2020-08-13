using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class EmailDetailStats
    {
        public List<Public.Email> NotOpenedList { get; }

        public List<Public.Email> OpenedList { get; }

        public EmailDetailStats(List<Public.Email> notOpenedList, List<Public.Email> openedList)
        {
            NotOpenedList = notOpenedList;
            OpenedList = openedList;
        }
    }
}