using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.App.Models.Api.v0.User;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    [ApiVersion("0")]
    [Route("/api/v0/users")]
    [ProducesErrorResponseType(typeof(void))]
    public class UserController : ControllerBase
    {
        protected BrowTricksPlatformConfiguration BrowTricksPlatformConfiguration { get; }

        protected IDbContext DbContext { get; }

        protected IUserService UserService { get; }

        public UserController(
            IDbContext dbContext,
            IUserService userService,
            IOptions<BrowTricksPlatformConfiguration> browTricksPlatfomrConfiguration)
        {
            DbContext = dbContext;
            UserService = userService;
            BrowTricksPlatformConfiguration = browTricksPlatfomrConfiguration.Value;
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] UserSearchModel model)
        {
            // Only browtricks has tenants
            var userQuery = DbContext.Users
                .Where(u => u.Tenant!.Company.Name == "Browtricks");
            if (!string.IsNullOrWhiteSpace(model.Query))
            {
                userQuery = userQuery.Where(u =>
                    $"{u.FirstName} {u.LastName}".Contains(model.Query)
                    || u.Email.Contains(model.Query)
                    || u.UserName.Contains(model.Query)
                    || u.PhoneNumber.Contains(model.Query));
            }

            var users = await userQuery
                .Skip(100 * (model.Page - 1))
                .Take(100)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.PhoneNumber,
                    u.Email
                })
                .ToListAsync();
            return Ok(users);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("{userId}/resetPassword")]
        public async Task<IActionResult> ResetPassword(int userId)
        {
            // Enforce tenant ID lookup to force Browtricks users
            var user = await DbContext.Users
                .Where(u => u.Tenant!.Company.Name == "Browtricks" && u.Id == userId)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            await UserService.SendForgotPasswordAsync(new Identity.Models.ForgotPasswordModel { UserName = user.UserName, ReturnUrl = $"{BrowTricksPlatformConfiguration.BaseUrl}/reset" });
            return Ok();
        }
    }
}