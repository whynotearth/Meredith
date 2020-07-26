using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Stripe;
using WhyNotEarth.Meredith.Services;
using WhyNotEarth.Meredith.Stripe.Data;

namespace WhyNotEarth.Meredith.Stripe
{
    public class StripeCustomerService : StripeServiceBase, IStripeCustomerService
    {
        protected IDbContext MeredithDbContext { get; }

        public StripeCustomerService(IOptions<StripeOptions> stripeOptions,
            IDbContext meredithDbContext) : base(stripeOptions)
        {
            MeredithDbContext = meredithDbContext;
        }

        public async Task<string> AddCustomerAsync(string? email, string? description)
        {
            var customerService = new CustomerService();
            var customer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Email = email,
                Description = description
            }, GetRequestOptions());
            return customer.Id;
        }

        public async Task<string> AddCardAsync(string? customerId, string? token)
        {
            var cardService = new CardService();
            var card = await cardService.CreateAsync(customerId, new CardCreateOptions
            {
                Source = token
            }, GetRequestOptions());
            return card.Id;
        }

        public async Task<string> AddTokenAsync(string? number, long expirationMonth, long expirationYear)
        {
            var tokenService = new TokenService();
            var token = await tokenService.CreateAsync(new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = number,
                    ExpMonth = expirationMonth,
                    ExpYear = expirationYear
                }
            }, GetRequestOptions());
            return token.Id;
        }

        public async Task DeleteCardAsync(string? customerId, string? cardId)
        {
            var cardService = new CardService();
            await cardService.DeleteAsync(customerId, cardId, null, GetRequestOptions());
        }
    }
}
