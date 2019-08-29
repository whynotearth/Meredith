namespace WhyNotEarth.Meredith.Tests.Stripe
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using WhyNotEarth.Meredith.Stripe;
    using WhyNotEarth.Meredith.Tests.Data;
    using Xunit;

    public class StripeServiceTests : DatabaseContextTest
    {
        public StripeService StripeServices { get; }

        public StripeServiceTests()
        {
            StripeServices = ServiceProvider.GetRequiredService<StripeService>();
        }
    }
}