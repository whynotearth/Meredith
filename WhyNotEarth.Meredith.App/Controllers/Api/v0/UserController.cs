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
            IOptions<BrowTricksPlatformConfiguration> browTricksPlatformConfiguration)
        {
            DbContext = dbContext;
            UserService = userService;
            BrowTricksPlatformConfiguration = browTricksPlatformConfiguration.Value;
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] UserSearchModel model)
        {
            // Only browtricks has tenants
            var userQuery = DbContext.Tenants
                .Where(t => t.Company.Name == "Browtricks")
                .Select(t => t.Owner)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(model.Query))
            {
                userQuery = userQuery.Where(u =>
                    (u.FirstName + " " + u.LastName).Contains(model.Query)
                    || u.Email.Contains(model.Query)
                    || u.UserName.Contains(model.Query)
                    || u.PhoneNumber.Contains(model.Query));
            }

            var users = await userQuery
                .Skip(100 * model.Page)
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
            var total = await userQuery.CountAsync();
            return Ok(
                new
                {
                    records = users,
                    total,
                    currentPage = model.Page,
                    perPage = 100,
                    pages = Math.Ceiling(total / 100m)
                });
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            // Only browtricks has tenants
            var user = await DbContext.Tenants
                .Where(t => t.Company.Name == "Browtricks")
                .Select(t => t.Owner)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.PhoneNumber,
                    u.Email
                })
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("{userId}/resetPassword")]
        public async Task<IActionResult> ResetPassword(int userId)
        {
            // Enforce tenant ID lookup to force Browtricks users
            var user = await DbContext.Tenants
                .Where(t => t.Company.Name == "Browtricks")
                .Select(t => t.Owner)
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            await UserService.SendForgotPasswordAsync(new Identity.Models.ForgotPasswordModel { UserName = user.UserName, ReturnUrl = $"{BrowTricksPlatformConfiguration.BaseUrl}/reset/{{userid}}", CompanySlug = "browtricks" });
            return Ok();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("{userId}/sendVerification")]
        public async Task<IActionResult> SendVerification(int userId)
        {
            // Enforce tenant ID lookup to force Browtricks users
            var user = await DbContext.Tenants
                .Where(t => t.Company.Name == "Browtricks")
                .Select(t => t.Owner)
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            if (user.EmailConfirmed)
            {
                return BadRequest(new { Message = "User already has a confirmed email" });
            }

            await UserService.SendConfirmEmailTokenAsync(user, new Identity.Models.ConfirmEmailTokenModel { ReturnUrl = $"{BrowTricksPlatformConfiguration.BaseUrl}/verify-submit-email", CompanySlug = "browtricks", });
            return Ok();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("sendVerification")]
        public async Task<IActionResult> SendVerificationAll()
        {
            // Enforce tenant ID lookup to force Browtricks users
            var users = await DbContext.Tenants
                .Where(t => t.Company.Name == "Browtricks")
                .Select(t => t.Owner)
                .Where(u => !u.EmailConfirmed)
                .ToListAsync();
            foreach (var user in users)
            {
                await UserService.SendConfirmEmailTokenAsync(user, new Identity.Models.ConfirmEmailTokenModel { ReturnUrl = $"{BrowTricksPlatformConfiguration.BaseUrl}/verify-submit-email", CompanySlug = "browtricks", });
            }

            return Ok();
        }
    }
}