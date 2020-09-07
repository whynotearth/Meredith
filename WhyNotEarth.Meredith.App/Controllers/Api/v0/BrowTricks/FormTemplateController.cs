using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks;
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
    [Route("api/v0/browtricks/tenants/{tenantSlug}/formtemplates")]
    public class FormTemplateController : BaseController
    {
        private readonly IFormTemplateService _formTemplateService;
        private readonly IUserService _userService;

        public FormTemplateController(IFormTemplateService formTemplateService, IUserService userService)
        {
            _formTemplateService = formTemplateService;
            _userService = userService;
        }

        [Returns201]
        [Returns404]
        [HttpPost("")]
        public async Task<CreateResult> Create(string tenantSlug, FormTemplateModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _formTemplateService.CreateAsync(tenantSlug, model, user);

            return Created();
        }

        [Returns204]
        [Returns404]
        [HttpPut("{templateId}")]
        public async Task<NoContentResult> Edit(string tenantSlug, int? templateId, FormTemplateModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _formTemplateService.EditAsync(tenantSlug, templateId, model, user);

            return NoContent();
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<FormTemplateResult>>> List(string tenantSlug)
        {
            var user = await GetCurrentUserAsync(_userService);

            var formTemplates = await _formTemplateService.GetListAsync(tenantSlug, user);

            return Ok(formTemplates.Select(item => new FormTemplateResult(item)));
        }

        [Returns200]
        [Returns404]
        [HttpGet("{templateId}")]
        public async Task<ActionResult<FormTemplateResult>> Get(int templateId)
        {
            var user = await GetCurrentUserAsync(_userService);

            var formTemplate = await _formTemplateService.GetAsync(templateId, user);

            return Ok(new FormTemplateResult(formTemplate));
        }

        [Returns204]
        [Returns404]
        [HttpDelete("{templateId}")]
        public async Task<NoContentResult> Delete(int templateId)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _formTemplateService.DeleteAsync(templateId, user);

            return NoContent();
        }
    }
}