using Microsoft.AspNetCore.Mvc;

namespace WhyNotEarth.Meredith.App.Mvc
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public CreateResult Created()
        {
            return new CreateResult();
        }

        [NonAction]
        public CreateObjectResult Created(object value)
        {
            return new CreateObjectResult(value);
        }
    }
}