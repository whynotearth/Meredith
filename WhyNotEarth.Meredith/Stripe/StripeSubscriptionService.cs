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

        public async Task<string> AddSubscriptionAsync(string customerId, string planId, string? coupon, decimal? feePercent, string? stripeAccountId)
        {
            var subscriptionService = new SubscriptionService();
            var stripeSubscription = await subscriptionService.CreateAsync(new SubscriptionCreateOptions
            {
                ApplicationFeePercent = feePercent,
                Customer = customerId,
                Coupon = coupon,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Plan = planId
                    }
                }

            }, GetRequestOptions(stripeAccountId));
            return stripeSubscription.Id;
        }

        public async Task CancelSubscriptionAsync(string subscriptionId, string? stripeAccountId)
        {
            var subscriptionService = new SubscriptionService();
            await subscriptionService.CancelAsync(subscriptionId, null, GetRequestOptions(stripeAccountId));
        }

        public async Task ChangeSubscriptionCardAsync(string subscriptionId, string cardId, string? stripeAccountId)
        {
            var subscriptionService = new SubscriptionService();
            await subscriptionService.UpdateAsync(subscriptionId, new SubscriptionUpdateOptions
            {
                DefaultPaymentMethod = cardId
            }, GetRequestOptions(stripeAccountId));
        }

        public async Task ChangeSubscriptionPlanAsync(string subscriptionId, string planId, decimal? feePercent, string? stripeAccountId)
        {
            var subscriptionService = new SubscriptionService();
            var subscriptionItemService = new SubscriptionItemService();
            var subscription = await subscriptionService.GetAsync(subscriptionId, null, GetRequestOptions(stripeAccountId));
            var itemToDeleteId = subscription.Items.First().Id;
            await subscriptionService.UpdateAsync(subscriptionId, new SubscriptionUpdateOptions
            {
                ApplicationFeePercent = feePercent,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Plan = planId
                    }
                },
            }, GetRequestOptions(stripeAccountId));
            await subscriptionItemService.DeleteAsync(itemToDeleteId, null, GetRequestOptions(stripeAccountId));
        }

        public async Task<string?> GetPriceByDescription(string description, string? stripeAccountId = null)
        {
            var priceService = new PriceService();
            var prices = await priceService.ListAsync(null, GetRequestOptions(stripeAccountId));
            var price = prices.FirstOrDefault(p => p.Nickname == description);
            if (price == null)
            {
                throw new Exception($"Price with description '{description}' could not be found on stripe account '{stripeAccountId}'");
            }

            return price.Id;
        }
    }
}
