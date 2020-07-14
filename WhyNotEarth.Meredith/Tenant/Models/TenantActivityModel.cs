using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Tenant.Models
{
    public class TenantActivityModel
    {
        [NotNull]
        [Mandatory]
        public bool? IsActive { get; set; }
    }
}