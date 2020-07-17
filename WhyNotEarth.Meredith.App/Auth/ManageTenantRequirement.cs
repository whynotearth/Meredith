using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Auth
{
    public class ManageTenantRequirement : IAuthorizationRequirement
    {
    }

    // Unfortunately, we can't access route values in the endpoint routing
    // so we can't get tenantSlug and check the actual owner of the tenant here
    // so we are just testing if the user has any tenants at all and actual permission check
    // is done in the services TenantService.CheckPermissionAsync()
    // https://github.com/dotnet/aspnetcore/issues/14442
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