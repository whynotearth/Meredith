using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace WhyNotEarth.Meredith.App
{
    public class Returns400Attribute : ProducesResponseTypeAttribute
    {
        public Returns400Attribute() : base(typeof(void), StatusCodes.Status400BadRequest)
        {
        }
    }
}