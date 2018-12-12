namespace WhyNotEarth.Meredith.App.Areas.Api.v1.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Stripe;

    [Route("/api/v1/stripe")]
    public class StripeController : Controller
    {
        protected StripeServices StripeServices { get; }
        
        public StripeController(StripeServices stripeServices)
        {
            StripeServices = stripeServices;
        }
        
        [Route("charge/create")]
        public async Task Create()
        {
            await StripeServices.CreateCharge(10000);
        }
    }
}