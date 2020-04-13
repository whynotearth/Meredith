using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen.Post;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/jumpstart")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageJumpStart)]
    public class JumpStartController : ControllerBase
    {
        private readonly JumpStartService _jumpStartService;

        public JumpStartController(JumpStartService jumpStartService)
        {
            _jumpStartService = jumpStartService;
        }

        [Returns400]
        [HttpPost("")]
        public async Task<IActionResult> Create(DateTime dateTime, List<int> postIds)
        {
            await _jumpStartService.CreateAsync(dateTime, postIds);

            return Ok();
        }

        [HttpPost("test")]
        public async Task<IActionResult> Create(TestModel model)
        {
            await _jumpStartService.SendTestAsync(model.Subject, model.Date, model.To, model.Description);

            return Ok();
        }
    }
}