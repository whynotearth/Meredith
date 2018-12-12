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
        
        public async Task CreateCharge(int amount)
        {
            var accountId = await MeredithDbContext.StripeAccounts.Select(s => s.StripeUserId).FirstOrDefaultAsync();
            var chargeService = new ChargeService();
            var charge = await chargeService.CreateAsync(new ChargeCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                SourceId = "tok_visa",
                ApplicationFee = (int)Math.Ceiling(amount * 0.12),
                Destination = new ChargeDestinationCreateOptions
                {
                    Account = accountId
                }
            }, GetRequestOptions());
        }
    }
}