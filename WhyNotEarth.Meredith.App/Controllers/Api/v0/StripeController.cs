namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models.Api.v0.Stripe;
    using Stripe;
    using WhyNotEarth.Meredith.Stripe.Data;

    [ApiVersion("0")]
    [Route("/api/v0/stripe")]
    [EnableCors]
    public class StripeController : Controller
    {
        public StripeController(
            StripeService stripeServices,
            IOptions<StripeOptions> stripeOptions,
            ILogger<StripeController> logger)
        {
            StripeOptions = stripeOptions.Value;
            StripeServices = stripeServices;
            Logger = logger;
        }

        protected StripeOptions StripeOptions { get; }

        protected StripeService StripeServices { get; }

        protected ILogger<StripeController> Logger { get; }

        [HttpGet]
        [Route("platform/keys")]
        public IActionResult GetKeys()
        {
            return Ok(new
            {
                StripeOptions.PublishableKey
            });
        }

        [HttpPost]
        [Route("paymentintent/create")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentIntentModel model)
        {
            try
            {
                var intent = await StripeServices.CreatePaymentIntent(model.CompanyId, model.Amount, model.Email, model.Metadata);
                return Ok(new { status = "success", intent });
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

        }

        [HttpPost]
        [Route("charge/create")]
        public async Task<IActionResult> Create([FromBody] CreateChargeModel model)
        {
            try
            {
                await StripeServices.CreateCharge(model.CompanyId, model.Token, model.Amount, model.Email,
                    model.Metadata);
                return Ok(new { status = "success" });
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
        }
    }
}