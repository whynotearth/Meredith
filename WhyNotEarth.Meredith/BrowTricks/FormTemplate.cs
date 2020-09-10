using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public class FormTemplate
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public Public.Tenant Tenant { get; set; } = null!;

        public string Name { get; set; } = null!;

        public FormTemplateType Type { get; set; }

        public List<FormItem>? Items { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

    public enum FormTemplateType : byte
    {
        [EnumMember(Value = "disclosure")]
        Disclosure = 1,

        [EnumMember(Value = "aftercare")]
        Aftercare = 2,

        [EnumMember(Value = "cancellation")]
        Cancellation = 3,

        [EnumMember(Value = "custom")]
        Custom = 4
    }
}