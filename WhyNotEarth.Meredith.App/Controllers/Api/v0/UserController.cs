using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    [ApiVersion("0")]
    [Route("/api/v0/users")]
    [ProducesErrorResponseType(typeof(void))]
    public class UserController : ControllerBase
    {
        protected IDbContext DbContext { get; }

        protected IUserService UserService { get; }

        public UserController(
            IDbContext dbContext,
            IUserService userService)
        {
            DbContext = dbContext;
            UserService = userService;
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Only browtricks has tenants
            var users = await DbContext.Users
                .Where(u => u.TenantId != null)
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
                .Where(u => u.TenantId != null && u.Id == userId)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            await UserService.SendForgotPasswordAsync(new Identity.Models.ForgotPasswordModel { UserName = user.UserName, ReturnUrl = "https://browtricksbeauty.com/reset" });
            return Ok();
        }
    }
}