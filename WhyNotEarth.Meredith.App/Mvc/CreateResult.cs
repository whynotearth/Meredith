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

    public class CreateObjectResult : ObjectResult
    {
        public CreateObjectResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.Created;
        }
    }
}