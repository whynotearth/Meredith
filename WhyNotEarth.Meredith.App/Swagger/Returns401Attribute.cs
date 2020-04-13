using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace WhyNotEarth.Meredith.App
{
    public class Returns401Attribute : ProducesResponseTypeAttribute
    {
        public Returns401Attribute() : base(typeof(void), StatusCodes.Status401Unauthorized)
        {
        }
    }
}