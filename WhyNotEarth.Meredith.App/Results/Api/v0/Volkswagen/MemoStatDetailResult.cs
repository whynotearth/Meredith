using System.Collections.Generic;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class MemoStatDetailResult
    {
        public MemoStatResult MemoStat { get; }

        public List<EmailRecipientResult> NotOpened { get; } = new List<EmailRecipientResult>();

        public List<EmailRecipientResult> Opened { get; } = new List<EmailRecipientResult>();

        public MemoStatDetailResult(MemoInfo memoInfo)
        {
            MemoStat = new MemoStatResult(memoInfo);
        }
    }
}