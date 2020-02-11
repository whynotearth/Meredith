namespace WhyNotEarth.Meredith.Stripe
{
    using Data;
    using global::Stripe;
    using Microsoft.Extensions.Options;

    public abstract class StripeServiceBase
    {
        protected StripeOptions StripeOptions { get; }

        protected StripeServiceBase(IOptions<StripeOptions> stripeOptions)
        {
            StripeOptions = stripeOptions.Value;
        }

        protected RequestOptions GetRequestOptions(string stripeAccount = null)
        {
            return new RequestOptions
            {
                ApiKey = StripeOptions.ClientSecret,
                StripeAccount = stripeAccount
            };
        }
    }
}