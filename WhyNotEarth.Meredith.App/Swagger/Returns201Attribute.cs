using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace WhyNotEarth.Meredith.App
{
    public class Returns201Attribute : ProducesResponseTypeAttribute
    {
        public Returns201Attribute() : base(typeof(void), StatusCodes.Status201Created)
        {
        }
    }
}