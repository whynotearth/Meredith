using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;
using WhyNotEarth.Meredith.Services.Models;

namespace WhyNotEarth.Meredith.Services
{
    public interface IStripeCustomerService
    {
        Task<string> AddCustomerAsync(string? email, string? description, string? stripeAccountId = null);

        Task<StripeCardDetail> AddCardAsync(string? customerId, string? token, string? stripeAccountId = null);

        Task DeleteCardAsync(string? customerId, string? cardId, string? stripeAccountId = null);

        Task<string> AddTokenAsync(string? number, long expirationMonth, long expirationYear, string? stripeAccountId = null);

        Task<List<Invoice>> GetInvoices(string customerId, string stripeAccountId);
    }
}