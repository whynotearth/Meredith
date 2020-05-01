using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/jumpstart")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class JumpStartController : ControllerBase
    {
        private readonly JumpStartService _jumpStartService;

        public JumpStartController(JumpStartService jumpStartService)
        {
            _jumpStartService = jumpStartService;
        }

        [Returns200]
        [Returns400]
        [HttpPost("")]
        public async Task<IActionResult> Create(JumpStartModel model)
        {
            await _jumpStartService.CreateAsync(model.DateTime, model.DistributionGroups, model.PostIds);

            return Ok();
        }
    }
}