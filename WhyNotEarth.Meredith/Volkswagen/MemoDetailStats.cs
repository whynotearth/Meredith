using System.Collections.Generic;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class MemoDetailStats
    {
        public List<MemoRecipient> NotOpenedList { get; }

        public List<MemoRecipient> OpenedList { get; }

        public MemoDetailStats(List<MemoRecipient> notOpenedList, List<MemoRecipient> openedList)
        {
            NotOpenedList = notOpenedList;
            OpenedList = openedList;
        }
    }
}