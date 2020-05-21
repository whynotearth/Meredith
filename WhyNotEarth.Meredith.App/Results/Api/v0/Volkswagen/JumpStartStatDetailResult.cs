using System.Collections.Generic;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class JumpStartStatDetailResult
    {
        public JumpStartStatResult JumpStartStat { get; }

        public List<EmailRecipientResult> NotOpened { get; } = new List<EmailRecipientResult>();

        public List<EmailRecipientResult> Opened { get; } = new List<EmailRecipientResult>();

        public JumpStartStatDetailResult(JumpStartInfo jumpStartInfo)
        {
            JumpStartStat = new JumpStartStatResult(jumpStartInfo);
        }
    }
}