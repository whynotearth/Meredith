namespace WhyNotEarth.Meredith.Identity
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using WhyNotEarth.Meredith.Data.Entity.Models;

    public class SignInManager : SignInManager<User>
    {
        public new UserManager UserManager { get; }

        public SignInManager(
            UserManager userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<User> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger,
            IAuthenticationSchemeProvider schemes)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
            UserManager = userManager;
        }

        public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
            => throw new System.InvalidOperationException("PasswordSignInAsync with username/password not supported, need site scope");

        public override Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
            => throw new System.InvalidOperationException("ExternalLoginSignInAsync without a domain is not supported, need site scope");

        public async Task<SignInResult> PasswordSignInAsync(string siteDomain, string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            if (string.IsNullOrWhiteSpace(siteDomain))
            {
                throw new System.Exception("Site domain is required");
            }

            var user = await UserManager.FindBySite(siteDomain, userName);
            return await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }

        public async Task<SignInResult> ExternalLoginSignInAsync(string siteDomain, string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
        {
            var user = await UserManager.FindByLoginAsync(loginProvider, providerKey);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            var error = await PreSignInCheck(user);
            if (error != null)
            {
                return error;
            }
            return await SignInOrTwoFactorAsync(user, isPersistent, loginProvider, bypassTwoFactor);
        }

    }
}