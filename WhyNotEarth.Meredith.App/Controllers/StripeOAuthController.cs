using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Models.StripeOAuth;
using WhyNotEarth.Meredith.Stripe;

namespace WhyNotEarth.Meredith.App.Controllers
{
    [Route("/stripe/oauth")]
    [DisableCors]
    public class StripeOAuthController : Controller
    {
        private readonly StripeOAuthService _stripeOAuthServices;

        public StripeOAuthController(StripeOAuthService stripeOAuthServices)
        {
            _stripeOAuthServices = stripeOAuthServices;
        }

        [Route("register/{requestId}")]
        public IActionResult Register(Guid requestId)
        {
            return Redirect(_stripeOAuthServices.GetOAuthRegisterUrl(requestId));
        }

        [Route("authorize")]
        public async Task<IActionResult> Authorize([FromQuery] AuthorizeModel model)
        {
            if (!string.IsNullOrEmpty(model.Error))
            {
                throw new Exception($"Error while authorizing stripe, {model.Error} - {model.ErrorDescription}");
            }

            await _stripeOAuthServices.Register(Guid.Parse(model.State), model.Code);

            return Ok("Your Stripe account has been connected with Meredith, you can now close your browser.");
        }
    }
}