using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Auth
{
    public class ManageTenantRequirement : IAuthorizationRequirement
    {
    }

    public class ManageTenantHandler : AuthorizationHandler<ManageTenantRequirement>
    {
        private readonly MeredithDbContext _dbContext;
        private readonly IUserManager _userManager;

        public ManageTenantHandler(MeredithDbContext dbContext, IUserManager userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ManageTenantRequirement requirement)
        {
            var user = await _userManager.GetUserAsync(context.User);

            if (user is null)
            {
                return;
            }

            var hasAnyTenants = await _dbContext.Tenants.AnyAsync(item => item.UserId == user.Id);

            if (hasAnyTenants)
            {
                context.Succeed(requirement);
            }
        }
    }
}