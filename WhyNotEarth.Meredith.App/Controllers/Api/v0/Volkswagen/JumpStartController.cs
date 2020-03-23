using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [ApiVersion("0")]
    [Route("api/v0/Volkswagen/jumpstart")]
    public class JumpStartController : ControllerBase
    {
        private readonly JumpStartService _jumpStartService;

        public JumpStartController(JumpStartService jumpStartService)
        {
            _jumpStartService = jumpStartService;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Policy = Policies.ManageJumpStart)]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(DateTime dateTime, List<int> postIds)
        {
            await _jumpStartService.CreateAsync(dateTime, postIds);

            return Ok();
        }
    }
}