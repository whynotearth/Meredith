using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.Services
{
    public interface IStripeCustomerService
    {
        Task<string> AddCustomerAsync(string? email, string? description);

        Task<string> AddCardAsync(string? customerId, string? token);

        Task DeleteCardAsync(string? customerId, string? cardId);

        Task<string> AddTokenAsync(string? number, long expirationMonth, long expirationYear);
    }
}