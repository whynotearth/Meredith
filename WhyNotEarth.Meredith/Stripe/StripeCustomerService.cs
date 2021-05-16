using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Stripe;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Services;
using WhyNotEarth.Meredith.Services.Models;
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

        public async Task<string> AddCustomerAsync(string? email, string? description, string? stripeAccountId = null)
        {
            var customerService = new CustomerService();
            var customer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Email = email,
                Description = description
            }, GetRequestOptions(stripeAccountId));
            return customer.Id;
        }

        public async Task<StripeCardDetail> AddCardAsync(string? customerId, string? token, string? stripeAccountId = null)
        {
            var cardService = new CardService();
            var card = await cardService.CreateAsync(customerId, new CardCreateOptions
            {
                Source = token
            }, GetRequestOptions(stripeAccountId));

            return new StripeCardDetail
            {
                Brand = card.Brand,
                Id = card.Id,
                Last4 = card.Last4,
                ExpirationMonth = (byte)card.ExpMonth,
                ExpirationYear = (ushort)card.ExpYear
            };
        }

        public async Task<string> AddTokenAsync(string? number, long expirationMonth, long expirationYear, string? stripeAccountId = null)
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
            }, GetRequestOptions(stripeAccountId));
            return token.Id;
        }

        public async Task DeleteCardAsync(string? customerId, string? cardId, string? stripeAccountId = null)
        {
            var cardService = new CardService();
            var card = await MeredithDbContext.PlatformCards.FindAsync(cardId);
            if (card is null)
            {
                throw new RecordNotFoundException();
            }

            MeredithDbContext.PlatformCards.Remove(card);
            try
            {
                await cardService.DeleteAsync(customerId, cardId, null, GetRequestOptions(stripeAccountId));
            }
            catch (StripeException e) when (e.StripeResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Ignore 404s, delete the record
            }

            await MeredithDbContext.SaveChangesAsync();
        }

        public async Task<List<Charge>> GetTransactions(string customerId, string stripeAccountId)
        {
            var chargeService = new ChargeService();
            var charges = await chargeService.ListAsync(new ChargeListOptions
            {
                Customer = customerId
            }, GetRequestOptions(stripeAccountId));
            return charges.Data;
        }
    }
}
