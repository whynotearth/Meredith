namespace WhyNotEarth.Meredith.Platform.Subscriptions
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.Exceptions;
    using WhyNotEarth.Meredith.Public;
    using WhyNotEarth.Meredith.Services;
    using static WhyNotEarth.Meredith.Public.Subscription;

    public class SubscriptionService
    {
        public SubscriptionService(
            IDbContext meredithDbContext,
            IStripeSubscriptionService stripeSubscriptionService)
        {
            _meredithDbContext = meredithDbContext;
            _stripeSubscriptionService = stripeSubscriptionService;
        }

        private readonly IDbContext _meredithDbContext;

        private readonly IStripeSubscriptionService _stripeSubscriptionService;

        public async Task<Subscription> StartSubscriptionAsync(int customerId, int planId, string? couponCode = null)
        {
            var customer = await _meredithDbContext.PlatformCustomers.FindAsync(customerId);
            if (customer == null)
            {
                throw new RecordNotFoundException($"Customer {customerId} not found");
            }

            var card = await _meredithDbContext.PlatformCards.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (card == null)
            {
                throw new RecordNotFoundException($"No card found on Customer {customerId}");
            }

            var plan = await _meredithDbContext.PlatformPlans.FindAsync(planId);
            if (plan == null)
            {
                throw new RecordNotFoundException($"Plan {planId} not found");
            }

            var stripeSubscriptionId = await _stripeSubscriptionService.AddSubscriptionAsync(customer.StripeId, plan.StripeId, couponCode);
            var subscription = new Subscription
            {
                CustomerId = customerId,
                CardId = card.Id,
                PlanId = plan.Id,
                StripeId = stripeSubscriptionId,
                Status = SubscriptionStatuses.Active
            };
            _meredithDbContext.PlatformSubscriptions.Add(subscription);
            await _meredithDbContext.SaveChangesAsync();
            return subscription;
        }

        public async Task CancelSubscriptionAsync(int subscriptionId)
        {
            var subscription = await _meredithDbContext.PlatformSubscriptions.FindAsync(subscriptionId);
            if (subscription == null)
            {
                throw new RecordNotFoundException($"Subscription {subscriptionId} not found");
            }

            await _stripeSubscriptionService.CancelSubscriptionAsync(subscription.StripeId);
            subscription.Status = SubscriptionStatuses.Cancelled;
            await _meredithDbContext.SaveChangesAsync();
        }

        public async Task ChangeSubscriptionCardAsync(int subscriptionId, int cardId)
        {
            var subscription = await _meredithDbContext.PlatformSubscriptions.FindAsync(subscriptionId);
            var card = await _meredithDbContext.PlatformCards.FindAsync(cardId);
            if (subscription == null)
            {
                throw new RecordNotFoundException($"Subscription {subscriptionId} not found");
            }

            if (card == null)
            {
                throw new RecordNotFoundException($"Card {cardId} not found");
            }

            await _stripeSubscriptionService.ChangeSubscriptionCardAsync(subscription.StripeId, card.StripeId);
            subscription.CardId = card.Id;
            await _meredithDbContext.SaveChangesAsync();
        }

        public async Task ChangeSubscriptionPlanAsync(int subscriptionId, int planId)
        {
            var subscription = await _meredithDbContext.PlatformSubscriptions.FindAsync(subscriptionId);
            if (subscription == null)
            {
                throw new RecordNotFoundException($"Subscription {subscriptionId} not found");
            }

            var plan = await _meredithDbContext.PlatformPlans.FindAsync(planId);
            if (plan == null)
            {
                throw new RecordNotFoundException($"Plan {planId} not found");
            }

            await _stripeSubscriptionService.ChangeSubscriptionPlanAsync(subscription.StripeId, plan.StripeId);
            subscription.PlanId = plan.Id;
            await _meredithDbContext.SaveChangesAsync();
        }
    }
}