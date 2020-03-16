using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace WhyNotEarth.Meredith.App
{
    public class Returns200Attribute : ProducesResponseTypeAttribute
    {
        public Returns200Attribute() : base(typeof(void), StatusCodes.Status200OK)
        {
        }
    }
}