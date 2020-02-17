using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;

namespace WhyNotEarth.Meredith.Services
{
    public interface IStripeService
    {
        Task<PaymentIntent> CreatePaymentIntent(string accountId, decimal amount, string email,
            Dictionary<string, string> metadata);

        PaymentIntent ConfirmPaymentIntent(string json, string stripSignatureHeader);
    }
}