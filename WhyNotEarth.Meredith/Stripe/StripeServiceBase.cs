using Microsoft.Extensions.Options;
using Stripe;
using WhyNotEarth.Meredith.Stripe.Data;

namespace WhyNotEarth.Meredith.Stripe
{
    public abstract class StripeServiceBase
    {
        protected StripeOptions StripeOptions { get; }

        protected StripeServiceBase(IOptions<StripeOptions> stripeOptions)
        {
            StripeOptions = stripeOptions.Value;
        }

        protected RequestOptions GetRequestOptions(string? stripeAccountId = null)
        {
            return new RequestOptions
            {
                ApiKey = StripeOptions.ClientSecret,
                StripeAccount = stripeAccountId
            };
        }
    }
}