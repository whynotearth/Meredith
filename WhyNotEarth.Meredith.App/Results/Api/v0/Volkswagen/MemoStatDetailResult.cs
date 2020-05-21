using System.Collections.Generic;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class MemoStatDetailResult
    {
        // TODO: Rename to Memo
        public MemoStatResult MemoList { get; }

        public List<EmailRecipientResult> NotOpened { get; } = new List<EmailRecipientResult>();

        public List<EmailRecipientResult> Opened { get; } = new List<EmailRecipientResult>();

        public MemoStatDetailResult(MemoInfo memoInfo)
        {
            MemoList = new MemoStatResult(memoInfo);
        }
    }
}