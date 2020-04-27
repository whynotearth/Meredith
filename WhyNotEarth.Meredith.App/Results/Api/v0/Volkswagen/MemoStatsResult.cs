﻿using System;
using System.Collections.Generic;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class MemoStatsResult
    {
        public MemoResult Memo { get; }

        public List<MemoRecipientResult> NotOpened { get; } = new List<MemoRecipientResult>();

        public List<MemoRecipientResult> Opened { get; } = new List<MemoRecipientResult>();

        public MemoStatsResult(MemoInfo memoInfo)
        {
            Memo = new MemoResult(memoInfo);
        }
    }

    public class MemoRecipientResult
    {
        public string Email { get; }

        public DateTime? DeliverDateTime { get; }

        public DateTime? OpenDateTime { get; }

        public MemoRecipientResult(MemoRecipient memoRecipient)
        {
            Email = memoRecipient.Email;
            DeliverDateTime = memoRecipient.DeliverDateTime;
            OpenDateTime = memoRecipient.OpenDateTime;
        }
    }
}