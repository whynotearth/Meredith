using Hangfire.Annotations;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Models
{
    public class ClientModel
    {
        [NotNull]
        [Mandatory]
        public RegisterModel User { get; set; } = null!;
    }
}