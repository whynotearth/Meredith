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
        private readonly IUserService _userService;

        public ManageTenantHandler(MeredithDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ManageTenantRequirement requirement)
        {
            var user = await _userService.GetUserAsync(context.User);

            if (user is null)
            {
                return;
            }

            var hasAnyTenants = await _dbContext.Tenants.AnyAsync(item => item.OwnerId == user.Id);

            if (hasAnyTenants)
            {
                context.Succeed(requirement);
            }
        }
    }
}