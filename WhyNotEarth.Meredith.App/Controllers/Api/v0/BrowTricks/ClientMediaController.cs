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
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageTenant)]
    [Route("api/v0/browtricks/tenants/{tenantSlug}/media")]
    public class ClientMediaController : BaseController
    {
        private readonly IClientMediaService _clientMediaService;
        private readonly IUserService _userService;

        public ClientMediaController(IClientMediaService clientMediaService, IUserService userService)
        {
            _clientMediaService = clientMediaService;
            _userService = userService;
        }

        [Returns201]
        [Returns404]
        [HttpPost("images")]
        public async Task<CreateResult> CreateImage(ClientImageModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientMediaService.CreateImageAsync(model, user);

            return Created();
        }

        [Returns204]
        [Returns404]
        [HttpDelete("images/{imageId}")]
        public async Task<NoContentResult> DeleteImage(int imageId)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientMediaService.DeleteImageAsync(imageId, user);

            return NoContent();
        }

        [Returns201]
        [Returns404]
        [HttpPost("videos")]
        public async Task<CreateResult> CreateVideo(ClientVideoModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientMediaService.CreateVideoAsync(model, user);

            return Created();
        }

        [Returns204]
        [Returns404]
        [HttpDelete("videos/{videoId}")]
        public async Task<NoContentResult> DeleteVideo(int videoId)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientMediaService.DeleteVideoAsync(videoId, user);

            return NoContent();
        }
    }
}