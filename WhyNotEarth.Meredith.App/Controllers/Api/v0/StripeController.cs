namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.Api.v0.Stripe;
    using Stripe;

    [ApiVersion("0")]
    [Route("/api/v0/stripe")]
    [EnableCors]
    public class StripeController : Controller
    {
        protected StripeService StripeServices { get; }

        protected ILogger<StripeController> Logger { get; }

        public StripeController(StripeService stripeServices,
            ILogger<StripeController> logger)
        {
            StripeServices = stripeServices;
            Logger = logger;
        }

        [HttpPost]
        [Route("paymentintent/create")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentIntentModel model)
        {
            try
            {
                await StripeServices.CreatePaymentIntent(model.CompanyId, model.Amount, model.Email, model.Metadata);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Caught exception");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    status = "error",
                    error = exception.Message
                });
            }

            return Ok(new { status = "success" });
        }

        [HttpPost]
        [Route("charge/create")]
        public async Task<IActionResult> Create([FromBody] CreateChargeModel model)
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
                    status = "error",
                    error = exception.Message
                });
            }

            return Ok(new { status = "success" });
        }
    }
}