using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.Services
{
    public interface IStripeSubscriptionService
    {
        Task<string> AddSubscriptionAsync(string customerId, string planId, string? coupon, decimal? feePercent, string? stripeAccountId);

        Task CancelSubscriptionAsync(string subscriptionId, string? stripeAccountId);

        Task ChangeSubscriptionCardAsync(string subscriptionId, string cardId, string? stripeAccountId);

        Task ChangeSubscriptionPlanAsync(string subscriptionId, string planId, decimal? feePercent, string? stripeAccountId);

        Task<string?> GetPriceByDescription(string description, string? stripeAccountId);
    }
}