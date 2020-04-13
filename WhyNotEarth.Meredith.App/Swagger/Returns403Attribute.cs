using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace WhyNotEarth.Meredith.App
{
    public class Returns403Attribute : ProducesResponseTypeAttribute
    {
        public Returns403Attribute() : base(typeof(void), StatusCodes.Status403Forbidden)
        {
        }
    }
}