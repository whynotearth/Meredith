using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    [Route("api/v0/browtricks/tenants/{tenantSlug}/formtemplates/{templateId}")]
    public class FormAnswerController : BaseController
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IFormAnswerService _formAnswerService;
        private readonly IUserService _userService;

        public FormAnswerController(IFormAnswerService formAnswerService, IUserService userService,
            IWebHostEnvironment environment)
        {
            _formAnswerService = formAnswerService;
            _userService = userService;
            _environment = environment;
        }

        [Authorize]
        [Returns404]
        [HttpGet("preview/png")]
        public async Task<IActionResult> PreviewPng(int templateId)
        {
            var user = await GetCurrentUserAsync(_userService);

            var data = await _formAnswerService.GetPngAsync(templateId, user);

            return Based64Png(data, _environment);
        }

        [Authorize]
        [Returns404]
        [HttpGet("preview/pdf")]
        public async Task<IActionResult> PreviewPdf(int templateId)
        {
            var user = await GetCurrentUserAsync(_userService);

            var data = await _formAnswerService.GetPdfAsync(templateId, user);

            return Based64Pdf(data, _environment);
        }

        [Authorize]
        [Returns404]
        [HttpPost("answer/preview")]
        public async Task<IActionResult> PreviewAnswer(int templateId, FormSignatureModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            var data = await _formAnswerService.GetPngAsync(templateId, model, user);

            return Based64Png(data, _environment);
        }

        [Authorize]
        [Returns404]
        [HttpPost("preview/{clientId}")]
        public async Task<IActionResult> PreviewClientAnswer(int templateId, int clientId, FormSignatureModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            var data = await _formAnswerService.GetPngAsync(templateId, clientId, model, user);

            return Based64Png(data, _environment);
        }

        [Authorize]
        [HttpPost("answer/{clientId}")]
        public async Task<NoContentResult> Submit(int templateId, int clientId, FormSignatureModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _formAnswerService.SubmitAsync(templateId, clientId, model, user);

            return NoContent();
        }

        [Returns404]
        [HttpPost("notify/{clientId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<OkResult> Notify(int templateId, int clientId, [FromQuery] string callbackUrl)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _formAnswerService.SendNotificationAsync(templateId, clientId, user, callbackUrl);

            return Ok();
        }
    }
}