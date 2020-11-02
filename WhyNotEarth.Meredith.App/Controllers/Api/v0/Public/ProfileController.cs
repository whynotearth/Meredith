using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Profile;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Identity.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [Authorize]
    [ApiVersion("0")]
    [Route("/api/v0/profile")]
    [ProducesErrorResponseType(typeof(void))]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public ProfileController(UserManager<User> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }

        [HttpGet("")]
        public async Task<ProfileResult> Get()
        {
            var user = await _userManager.GetUserAsync(User);

            return new ProfileResult(user);
        }

        [Returns204]
        [HttpPut("")]
        public async Task<IActionResult> Update(ProfileModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var identityResult = await _userService.UpdateUserAsync(user.Id.ToString(), model);

            if (!identityResult.Succeeded)
            {
                return Error(identityResult.Errors);
            }

            return NoContent();
        }

        private BadRequestObjectResult Error(IEnumerable<IdentityError> identityErrors)
        {
            var errors = string.Join(",", identityErrors.Select(e => e.Description).ToList());

            return BadRequest(new { error = errors });
        }
    }
}