namespace WhyNotEarth.Meredith.Identity
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Data.Entity.Models;

    public class UserStore : UserStore<User, Role, MeredithDbContext, int, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>, IUserStore
    {
        public UserStore(MeredithDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        public async Task<User> FindByDomainAsync(string domain, string userName)
        {
            return await Context.Users
                .FirstOrDefaultAsync(u => u.Site.Domain == domain && u.UserName == userName);
        }

        public async Task<User> FindByLoginDomainAsync(string domain, string loginProvider, string providerKey)
        {
            var userLogin = await Context.UserLogins
                .SingleOrDefaultAsync(userLogin =>
                    userLogin.Site.Domain == domain
                    && userLogin.LoginProvider == loginProvider
                    && userLogin.ProviderKey == providerKey);
            return userLogin == null ? null : await FindUserAsync(userLogin.UserId, default);
        }
    }
}