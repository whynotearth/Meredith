namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Models.Api.v0.Stripe;
    using Stripe;

    [ApiVersion("0")]
    [Route("/api/v0/stripe")]
    public class StripeController : Controller
    {
        protected StripeServices StripeServices { get; }
        
        public StripeController(StripeServices stripeServices)
        {
            StripeServices = stripeServices;
        }
        
        [HttpPost]
        [Route("charge/create")]
        public async Task Create([FromBody] CreateModel model)
        {
            await StripeServices.CreateCharge(model.CompanyId, model.Token, model.Amount, model.Email, model.Metadata);
        }
    }
}