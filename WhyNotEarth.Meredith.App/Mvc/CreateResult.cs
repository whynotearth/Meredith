using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace WhyNotEarth.Meredith.App.Mvc
{
    public class CreateResult : StatusCodeResult
    {
        public CreateResult() : base((int)HttpStatusCode.Created)
        {
        }
    }
}