// ReSharper disable InconsistentNaming

namespace WhyNotEarth.Meredith.App.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Areas.Stripe.StripeOAuth;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Stripe;

    [Route("/stripe/oauth")]
    [DisableCors]
    public class StripeOAuthController : ControllerBase
    {
        protected StripeOAuthService StripeOAuthServices { get; }

        public StripeOAuthController(StripeOAuthService stripeOAuthServices)
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

            return Ok("Your Stripe account has been connected with Meredith, you can now close your browser.");
        }
    }
}