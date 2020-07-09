using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class ClientNoteModel
    {
        public int? Id { get; set; }

        [NotNull]
        [Mandatory]
        public string? Note { get; set; }
    }
}