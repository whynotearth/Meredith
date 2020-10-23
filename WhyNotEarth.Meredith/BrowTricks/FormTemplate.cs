using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public class FormTemplate
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public Public.Tenant Tenant { get; set; } = null!;

        public string Name { get; set; } = null!;

        public List<FormItem> Items { get; set; } = new List<FormItem>();

        public DateTime? CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}