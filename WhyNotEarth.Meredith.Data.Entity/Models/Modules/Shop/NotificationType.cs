using System;
using System.Runtime.Serialization;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    [Flags]
    public enum NotificationType : byte
    {
        [EnumMember(Value = "none")]
        None = 0,
        
        [EnumMember(Value = "email")]
        Email = 1,

        [EnumMember(Value = "whatsapp")]
        Whatsapp = 2,

        [EnumMember(Value = "text")]
        Text = 4
    }
}