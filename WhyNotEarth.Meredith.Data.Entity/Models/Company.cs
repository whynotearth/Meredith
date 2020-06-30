#nullable enable

using System.Collections.Generic;
using System.Diagnostics;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    [DebuggerDisplay("{" + nameof(Slug) + "}")]
    public class Company
    {
        public int Id { get; set; }

        public string Slug { get; set; } = null!;

        public string Name { get; set; } = null!;

        public ICollection<Page>? Pages { get; set; }

        public ICollection<Tenant>? Tenants { get; set; }
    }
}