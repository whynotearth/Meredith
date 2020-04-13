using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace WhyNotEarth.Meredith.App
{
    public class Returns204Attribute : ProducesResponseTypeAttribute
    {
        public Returns204Attribute() : base(typeof(void), StatusCodes.Status204NoContent)
        {
        }
    }
}