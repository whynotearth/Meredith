// ReSharper disable InconsistentNaming

namespace WhyNotEarth.Meredith.App.Areas.Api.v1.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Models.StripeOAuth;
    using Stripe;

    [Route("/api/v1/stripe/oauth")]
    public class StripeOAuthController : Controller
    {
        protected StripeOAuthServices StripeOAuthServices { get; }

        public StripeOAuthController(StripeOAuthServices stripeOAuthServices)
        {
            StripeOAuthServices = stripeOAuthServices;
        }

        [Route("register/{requestId}")]
        public IActionResult Register(Guid requestId)
        {
            return Redirect(StripeOAuthServices.GetOAuthRegisterUrl(requestId));
        }

        [Route("authorize")]
        public async Task<IActionResult> Authorize(AuthorizeModel model)
        {
            if (!string.IsNullOrEmpty(model.Error))
            {
                throw new Exception($"Error while authorizing stripe, {model.Error} - {model.ErrorDescription}");
            }

            await StripeOAuthServices.Register(Guid.Parse(model.State), model.Code);
            return Ok();
        }
    }
}