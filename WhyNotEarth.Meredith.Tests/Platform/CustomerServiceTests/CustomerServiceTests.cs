namespace WhyNotEarth.Meredith.Tests.Platform.SubscriptionServiceTests
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
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
            var card = await CreateCard();
            var databaseCard = await DbContext.PlatformCards.AsNoTracking().FirstOrDefaultAsync(c => c.Id == card.Id);
            Assert.Equal("1111", databaseCard.Last4);
            Assert.Equal(PaymentCard.Brands.Visa, databaseCard.Brand);
            Assert.Equal(1, databaseCard.ExpirationMonth);
            Assert.Equal(2050, databaseCard.ExpirationYear);
        }

        [Fact]
        public async Task RemoveCard()
        {
            var card = await CreateCard();
            await CustomerService.DeleteCardAsync(card.Id);
            var databaseCard = await DbContext.PlatformCards.AsNoTracking().FirstOrDefaultAsync(c => c.Id == card.Id);
            Assert.Null(databaseCard);
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