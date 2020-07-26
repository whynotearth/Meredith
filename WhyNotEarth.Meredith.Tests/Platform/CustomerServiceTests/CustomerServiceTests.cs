namespace WhyNotEarth.Meredith.Tests.Platform.SubscriptionServiceTests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using WhyNotEarth.Meredith.Platform.Subscriptions;
    using WhyNotEarth.Meredith.Public;
    using WhyNotEarth.Meredith.Services;
    using Xunit;

    public class CustomerServiceTests : BasePlatformTests
    {
        public CustomerServiceTests()
        {
            CustomerService = ServiceProvider.GetRequiredService<CustomerService>();
            StripeCustomerService = ServiceProvider.GetRequiredService<IStripeCustomerService>();
        }

        protected CustomerService CustomerService { get; }

        protected IStripeCustomerService StripeCustomerService { get; }

        [Fact]
        public async Task AddCustomer()
        {
            var tenant = await CreateTenant();
            await CustomerService.AddCustomerAsync(tenant.Id);
        }

        [Fact]
        public async Task AddCard()
        {
            await CreateCard();
        }

        [Fact]
        public async Task RemoveCard()
        {
            var card = await CreateCard();
            await CustomerService.DeleteCardAsync(card.Id);
        }

        protected async Task<PaymentCard> CreateCard()
        {
            var tenant = await CreateTenant();
            await CustomerService.AddCustomerAsync(tenant.Id);
            var token = await StripeCustomerService.AddTokenAsync("4111111111111111", 1, 2050);
            return await CustomerService.AddCardAsync(tenant.Id, token);
        }
    }
}