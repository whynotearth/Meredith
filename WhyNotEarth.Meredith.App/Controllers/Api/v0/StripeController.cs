namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.Api.v0.Stripe;
    using Stripe;

    [ApiVersion("0")]
    [Route("/api/v0/stripe")]
    public class StripeController : Controller
    {
        protected StripeServices StripeServices { get; }
        
        protected ILogger<StripeController> Logger { get; }

        public StripeController(StripeServices stripeServices,
            ILogger<StripeController> logger)
        {
            StripeServices = stripeServices;
            Logger = logger;
        }
        
        [HttpPost]
        [Route("charge/create")]
        public async Task<IActionResult> Create([FromBody] CreateModel model)
        {
            try
            {
                await StripeServices.CreateCharge(model.CompanyId, model.Token, model.Amount, model.Email,
                    model.Metadata);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Caught exception");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = exception.Message
                });
            }

            return Ok();
        }
    }
}