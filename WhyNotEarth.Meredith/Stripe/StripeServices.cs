namespace WhyNotEarth.Meredith.Stripe
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using global::Stripe;
    using Meredith.Data.Entity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class StripeServices : StripeServiceBase
    {
        protected MeredithDbContext MeredithDbContext { get; }

        public StripeServices(IOptions<StripeOptions> stripeOptions,
            MeredithDbContext meredithDbContext) : base(stripeOptions)
        {
            MeredithDbContext = meredithDbContext;
        }
        
        public async Task CreateCharge(Guid companyId, string token, decimal amount)
        {
            var accountId = await MeredithDbContext.StripeAccounts
                .Where(s => s.CompanyId == companyId)
                .Select(s => s.StripeUserId)
                .FirstOrDefaultAsync();
            var chargeService = new ChargeService();
            await chargeService.CreateAsync(new ChargeCreateOptions
            {
                Amount = (int)(amount * 100),
                Currency = "usd",
                SourceId = token,
                ApplicationFee = (int)Math.Ceiling(amount * 0.12m),
                Destination = new ChargeDestinationCreateOptions
                {
                    Account = accountId
                }
            }, GetRequestOptions());
        }
    }
}