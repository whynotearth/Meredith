using System.Collections.Generic;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class EmailDetailStats
    {
        public List<Data.Entity.Models.Email> NotOpenedList { get; }

        public List<Data.Entity.Models.Email> OpenedList { get; }

        public EmailDetailStats(List<Data.Entity.Models.Email> notOpenedList, List<Data.Entity.Models.Email> openedList)
        {
            NotOpenedList = notOpenedList;
            OpenedList = openedList;
        }
    }
}