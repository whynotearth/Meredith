using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.Services
{
    public interface IStripeSubscriptionService
    {
        Task<string> AddSubscriptionAsync(string customerId, string planId, string? coupon);

        Task CancelSubscriptionAsync(string subscriptionId);

        Task ChangeSubscriptionCardAsync(string subscriptionId, string cardId);

        Task ChangeSubscriptionPlanAsync(string subscriptionId, string planId);

        Task<string?> GetPriceByDescription(string description);
    }
}