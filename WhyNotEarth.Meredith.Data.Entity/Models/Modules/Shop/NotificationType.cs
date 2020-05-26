using System;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    [Flags]
    public enum NotificationType : byte
    {
        None = 0,
        Email = 1,
        Whatsapp = 2,
        Text = 4
    }
}