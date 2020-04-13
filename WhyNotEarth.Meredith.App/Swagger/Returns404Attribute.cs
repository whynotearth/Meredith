using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace WhyNotEarth.Meredith.App
{
    public class Returns404Attribute : ProducesResponseTypeAttribute
    {
        public Returns404Attribute() : base(typeof(void), StatusCodes.Status404NotFound)
        {
        }
    }
}