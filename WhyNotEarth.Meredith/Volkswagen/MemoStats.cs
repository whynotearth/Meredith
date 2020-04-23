using System.Collections.Generic;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class MemoStats
    {
        public List<MemoRecipient> NotOpenedList { get; }

        public List<MemoRecipient> OpenedList { get; }

        public MemoStats(List<MemoRecipient> notOpenedList, List<MemoRecipient> openedList)
        {
            NotOpenedList = notOpenedList;
            OpenedList = openedList;
        }
    }
}