namespace WhyNotEarth.Meredith.Stripe
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using global::Stripe;
    using Meredith.Data.Entity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class StripeService : StripeServiceBase
    {
        protected MeredithDbContext MeredithDbContext { get; }

        public StripeService(IOptions<StripeOptions> stripeOptions,
            MeredithDbContext meredithDbContext) : base(stripeOptions)
        {
            MeredithDbContext = meredithDbContext;
        }

        public async Task<PaymentIntent> CreatePaymentIntent(
            int companyId,
            decimal amount,
            string email,
            Dictionary<string, string> metadata)
        {
            var accountId = await GetAccountFromCompany(companyId);
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = (int)(amount * 100),
                Currency = "usd",
                ApplicationFeeAmount = (int)Math.Ceiling(amount * 0.12m),
                Metadata = metadata,
                ReceiptEmail = email,
                TransferData = new PaymentIntentTransferDataOptions
                {
                    Destination = accountId
                }
            }, GetRequestOptions());
            return paymentIntent;
        }

        public async Task<PaymentIntent> ConfirmPaymentIntent(string intentId)
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.ConfirmAsync(intentId, new PaymentIntentConfirmOptions
            {

            }, GetRequestOptions());
            return paymentIntent;
        }

        public async Task<string> CreateCharge(int companyId, string token, decimal amount, string email,
            Dictionary<string, string> metadata, bool capture = true)
        {
            var accountId = await GetAccountFromCompany(companyId);
            var chargeService = new ChargeService();
            var charge = await chargeService.CreateAsync(new ChargeCreateOptions
            {
                Amount = (int)(amount * 100),
                Currency = "usd",
                Source = token,
                ApplicationFeeAmount = (int)Math.Ceiling(amount * 0.12m),
                Destination = new ChargeDestinationCreateOptions
                {
                    Account = accountId
                },
                ReceiptEmail = email,
                Metadata = metadata,
                Capture = capture
            }, GetRequestOptions());
            return charge.Id;
        }

        public async Task<string> CreateAuthorization(int companyId, string token, decimal amount, string email,
            Dictionary<string, string> metadata)
        {
            return await CreateCharge(companyId, token, amount, email, metadata, false);
        }

        public async Task CaptureCharge(string chargeId)
        {
            var chargeService = new ChargeService();
            await chargeService.CaptureAsync(chargeId, new ChargeCaptureOptions());
        }

        private async Task<string> GetAccountFromCompany(int companyId)
        {
            var accountId = await MeredithDbContext.StripeAccounts
                .Where(s => s.CompanyId == companyId)
                .Select(s => s.StripeUserId)
                .FirstOrDefaultAsync();
            if (accountId == null)
            {
                if (await MeredithDbContext.Companies.AnyAsync(c => c.Id == companyId))
                {
                    throw new Exception($"Company {companyId} does not have Stripe configured");
                }
                else
                {
                    throw new Exception($"Company {companyId} not found");
                }
            }

            return accountId;
        }
    }
}