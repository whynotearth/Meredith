using System;
using System.Runtime.Serialization;

namespace WhyNotEarth.Meredith.Shop
{
    [Flags]
    public enum PaymentMethodType : byte
    {
        [EnumMember(Value = "cash")]
        Cash = 1,

        [EnumMember(Value = "abaBankTransfer")]
        AbaBankTransfer = 2
    }
}