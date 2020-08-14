using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Stripe.Data;

namespace WhyNotEarth.Meredith.Stripe
{
    public class StripeOAuthService : StripeServiceBase
    {
        private readonly IDbContext _dbContext;

        public StripeOAuthService(IOptions<StripeOptions> stripeOptions, IDbContext dbContext) : base(stripeOptions)
        {
            _dbContext = dbContext;
        }

        public string GetOAuthRegisterUrl(Guid requestId)
        {
            return
                $"https://connect.stripe.com/oauth/authorize?response_type=code&client_id={StripeOptions.ClientId}&scope=read_write&redirect_uri={StripeOptions.RedirectUri}&state={requestId}";
        }

        public async Task Register(Guid requestId, string code)
        {
            var request = await _dbContext.StripeOAuthRequests.FirstOrDefaultAsync(s => s.Id == requestId);
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
                await _dbContext.StripeAccounts.FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId);
            if (stripeAccount == null)
            {
                stripeAccount = new StripeAccount
                {
                    CompanyId = request.CompanyId
                };
                _dbContext.StripeAccounts.Add(stripeAccount);
            }

            stripeAccount.AccessToken = oAuthToken.AccessToken;
            stripeAccount.LiveMode = oAuthToken.Livemode;
            stripeAccount.RefreshToken = oAuthToken.RefreshToken;
            stripeAccount.TokenType = oAuthToken.TokenType;
            stripeAccount.StripePublishableKey = oAuthToken.StripePublishableKey;
            stripeAccount.StripeUserId = oAuthToken.StripeUserId;
            stripeAccount.Scope = oAuthToken.Scope;
            _dbContext.StripeOAuthRequests.Remove(request);
            await _dbContext.SaveChangesAsync();
        }
    }
}