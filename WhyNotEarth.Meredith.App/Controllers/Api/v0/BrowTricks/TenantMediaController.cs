using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.BrowTricks.Services;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.BrowTricks
{
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageTenant)]
    [Route("api/v0/browtricks/tenants/{tenantSlug}/media")]
    public class TenantMediaController : BaseController
    {
        private readonly ITenantMediaService _tenantMediaService;
        private readonly IUserService _userService;

        public TenantMediaController(ITenantMediaService tenantMediaService, IUserService userService)
        {
            _tenantMediaService = tenantMediaService;
            _userService = userService;
        }

        [Returns404]
        [HttpPost("images")]
        public async Task<CreateResult> CreateImage(string tenantSlug, BrowTricksImageModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _tenantMediaService.CreateImageAsync(tenantSlug, model, user);

            return Created();
        }

        [Returns404]
        [HttpDelete("images/{imageId}")]
        public async Task<NoContentResult> DeleteImage(int imageId)
        {
            var user = await _userService.GetUserAsync(User);

            await _tenantMediaService.DeleteImageAsync(imageId, user);

            return NoContent();
        }

        [Returns404]
        [HttpPost("videos")]
        public async Task<CreateResult> CreateVideo(string tenantSlug, BrowTricksVideoModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _tenantMediaService.CreateVideoAsync(tenantSlug, model, user);

            return Created();
        }

        [Returns404]
        [HttpDelete("videos/{videoId}")]
        public async Task<NoContentResult> DeleteVideo(int videoId)
        {
            var user = await _userService.GetUserAsync(User);

            await _tenantMediaService.DeleteVideoAsync(videoId, user);

            return NoContent();
        }
    }
}