namespace WhyNotEarth.Meredith.Stripe
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using global::Stripe;
    using Meredith.Data.Entity;
    using Meredith.Data.Entity.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class StripeOAuthServices : StripeServiceBase
    {
        protected MeredithDbContext MeredithDbContext { get; }


        public StripeOAuthServices(IOptions<StripeOptions> stripeOptions,
            MeredithDbContext meredithDbContext) : base(stripeOptions)
        {
            MeredithDbContext = meredithDbContext;
        }

        public string GetOAuthRegisterUrl(Guid requestId)
        {
            return
                $"https://connect.stripe.com/oauth/authorize?response_type=code&client_id={StripeOptions.ClientId}&scope=read_write&redirect_uri={StripeOptions.RedirectUri}&state={requestId}";
        }

        public async Task Register(Guid requestId, string code)
        {
            var request = await MeredithDbContext.StripeOAuthRequests.FirstOrDefaultAsync(s => s.Id == requestId);
            if (request == null)
            {
                throw new Exception("Invalid request ID");
            }

            var oAuthTokenService = new OAuthTokenService();
            var oAuthToken = await oAuthTokenService.CreateAsync(new OAuthTokenCreateOptions
            {
                ClientSecret = StripeOptions.ClientSecret,
                Code = code,
                GrantType = "authorization_code"
            }, GetRequestOptions());
            var stripeAccount =
                await MeredithDbContext.StripeAccounts.FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId);
            if (stripeAccount == null)
            {
                stripeAccount = new StripeAccount
                {
                    CompanyId = request.CompanyId
                };
                MeredithDbContext.StripeAccounts.Add(stripeAccount);
            }

            stripeAccount.AccessToken = oAuthToken.AccessToken;
            stripeAccount.LiveMode = oAuthToken.Livemode;
            stripeAccount.RefreshToken = oAuthToken.RefreshToken;
            stripeAccount.TokenType = oAuthToken.TokenType;
            stripeAccount.StripePublishableKey = oAuthToken.StripePublishableKey;
            stripeAccount.StripeUserId = oAuthToken.StripeUserId;
            stripeAccount.Scope = oAuthToken.Scope;
            MeredithDbContext.StripeOAuthRequests.Remove(request);
            await MeredithDbContext.SaveChangesAsync();
        }
    }
}