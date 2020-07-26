using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Stripe;
using WhyNotEarth.Meredith.Services;
using WhyNotEarth.Meredith.Stripe.Data;

namespace WhyNotEarth.Meredith.Stripe
{
    public class StripeSubscriptionService : StripeServiceBase, IStripeSubscriptionService
    {
        protected IDbContext MeredithDbContext { get; }

        public StripeSubscriptionService(IOptions<StripeOptions> stripeOptions,
            IDbContext meredithDbContext) : base(stripeOptions)
        {
            MeredithDbContext = meredithDbContext;
        }

        public async Task<string> AddSubscriptionAsync(string customerId, string planId, string? coupon)
        {
            var subscriptionService = new SubscriptionService();
            var stripeSubscription = await subscriptionService.CreateAsync(new SubscriptionCreateOptions
            {
                Customer = customerId,
                Coupon = coupon,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Plan = planId
                    }
                }

            }, GetRequestOptions());
            return stripeSubscription.Id;
        }

        public async Task CancelSubscriptionAsync(string subscriptionId)
        {
            var subscriptionService = new SubscriptionService();
            await subscriptionService.CancelAsync(subscriptionId, null, GetRequestOptions());
        }

        public async Task ChangeSubscriptionCardAsync(string subscriptionId, string cardId)
        {
            var subscriptionService = new SubscriptionService();
            await subscriptionService.UpdateAsync(subscriptionId, new SubscriptionUpdateOptions
            {
                DefaultPaymentMethod = cardId
            }, GetRequestOptions());
        }

        public async Task ChangeSubscriptionPlanAsync(string subscriptionId, string planId)
        {
            var subscriptionService = new SubscriptionService();
            var subscriptionItemService = new SubscriptionItemService();
            var subscription = await subscriptionService.GetAsync(subscriptionId, null, GetRequestOptions());
            var itemToDeleteId = subscription.Items.First().Id;
            await subscriptionService.UpdateAsync(subscriptionId, new SubscriptionUpdateOptions
            {
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Plan = planId
                    }
                },
            }, GetRequestOptions());
            await subscriptionItemService.DeleteAsync(itemToDeleteId, null, GetRequestOptions());
        }

        public async Task<string?> GetPriceByDescription(string description)
        {
            var priceService = new PriceService();
            var prices = await priceService.ListAsync(null, GetRequestOptions());
            return prices.FirstOrDefault(p => p.Nickname == description).Id;
        }
    }
}
