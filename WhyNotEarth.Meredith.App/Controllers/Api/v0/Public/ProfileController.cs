using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Profile;
using WhyNotEarth.Meredith.Identity.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [Authorize]
    [Returns401]
    [ApiVersion("0")]
    [Route("/api/v0/profile")]
    [ProducesErrorResponseType(typeof(void))]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public ProfileController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<ProfileResult>> Get()
        {
            var user = await _userManager.GetUserAsync(User);

            return Ok(new ProfileResult(user));
        }

        [Returns204]
        [HttpPut("")]
        public async Task<IActionResult> Update(ProfileModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.GoogleLocation = model.GoogleLocation;

            var identityResult = await _userManager.UpdateAsync(user);

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