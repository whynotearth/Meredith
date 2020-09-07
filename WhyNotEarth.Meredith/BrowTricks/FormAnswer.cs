using System.Collections.Generic;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public class FormAnswer
    {
        public int Id { get; set; }

        public int FormSignatureId { get; set; }

        public FormSignature FormSignature { get; set; } = null!;

        public FormItemType Type { get; set; }

        public bool IsRequired { get; set; }

        public string Question { get; set; } = null!;

        public List<string>? Options { get; set; }

        public List<string>? Answers { get; set; } = null!;
    }
}