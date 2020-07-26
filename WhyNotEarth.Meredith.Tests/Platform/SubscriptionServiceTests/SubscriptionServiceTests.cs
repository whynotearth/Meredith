namespace WhyNotEarth.Meredith.Tests.Platform.SubscriptionServiceTests
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using WhyNotEarth.Meredith.Platform.Subscriptions;
    using WhyNotEarth.Meredith.Public;
    using WhyNotEarth.Meredith.Services;
    using Xunit;
    using static WhyNotEarth.Meredith.Public.Subscription;

    public class SubscriptionServiceTests : BasePlatformTests
    {
        public SubscriptionServiceTests()
        {
            CustomerService = ServiceProvider.GetRequiredService<CustomerService>();
            StripeCustomerService = ServiceProvider.GetRequiredService<IStripeCustomerService>();
            SubscriptionService = ServiceProvider.GetRequiredService<SubscriptionService>();
        }

        protected CustomerService CustomerService { get; }

        protected IStripeCustomerService StripeCustomerService { get; }

        protected SubscriptionService SubscriptionService { get; }

        [Fact]
        public async Task AddSubscription()
        {
            var (_, subscriptionId, _) = await CreateSubscription();
            var subscription = await GetSubscriptionAsync(subscriptionId);
            Assert.Equal(SubscriptionStatuses.Active, subscription.Status);
        }

        [Fact]
        public async Task CancelSubscription()
        {
            var (_, subscriptionId, _) = await CreateSubscription();
            await SubscriptionService.CancelSubscriptionAsync(subscriptionId);
            var subscription = await GetSubscriptionAsync(subscriptionId);
            Assert.Equal(SubscriptionStatuses.Cancelled, subscription.Status);
        }

        [Fact]
        public async Task ChangeCardOnSubscription()
        {
            var (_, subscriptionId, tenantId) = await CreateSubscription();
            var token = await StripeCustomerService.AddTokenAsync("4111111111111111", 1, 2050);
            var card2 = await CustomerService.AddCardAsync(tenantId, token);
            await SubscriptionService.ChangeSubscriptionCardAsync(subscriptionId, card2.Id);
            var subscription = await GetSubscriptionAsync(subscriptionId);
            Assert.Equal(card2.Id, subscription.CardId);
        }

        [Fact]
        public async Task ChangeSubscriptionPlan()
        {
            var (_, subscriptionId, _) = await CreateSubscription();
            var plan = await GetPlan("Browtricks 2: Electric Boogaloo");
            await SubscriptionService.ChangeSubscriptionPlanAsync(subscriptionId, plan.Id);
            var subscription = await GetSubscriptionAsync(subscriptionId);
            Assert.Equal(plan.Id, subscription.PlanId);
        }

        protected async Task<(int customerId, int subscriptionI, int tenantId)> CreateSubscription()
        {
            var tenant = await CreateTenant();
            var plan = await GetPlan("Browtricks");
            var customer = await CustomerService.AddCustomerAsync(tenant.Id);
            var token = await StripeCustomerService.AddTokenAsync("4111111111111111", 1, 2050);
            await CustomerService.AddCardAsync(tenant.Id, token);
            var subscription = await SubscriptionService.StartSubscriptionAsync(customer.Id, plan.Id, "fiveoff");
            return (customer.Id, subscription.Id, tenant.Id);
        }

        protected async Task<Subscription> GetSubscriptionAsync(int subscriptionId)
        {
            return await DbContext.PlatformSubscriptions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == subscriptionId);
        }
    }
}