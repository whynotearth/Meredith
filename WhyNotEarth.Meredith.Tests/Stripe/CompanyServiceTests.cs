namespace WhyNotEarth.Meredith.Tests.Stripe
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using WhyNotEarth.Meredith.Stripe;
    using WhyNotEarth.Meredith.Tests.Data;
    using Xunit;

    public class StripeServiceTests : DatabaseContextTest
    {
        public StripeServices StripeServices { get; }

        public StripeServiceTests()
        {
            StripeServices = ServiceProvider.GetRequiredService<StripeServices>();
        }

        [Fact]
        public async Task Test()
        {

        }
    }
}