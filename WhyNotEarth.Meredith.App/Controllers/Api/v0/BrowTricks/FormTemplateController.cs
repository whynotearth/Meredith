﻿using System.Collections.Generic;
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
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
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

        [Returns404]
        [HttpPost("")]
        [ProducesResponseType(typeof(int), 201)]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<int> Create(string tenantSlug, FormTemplateModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            var id = await _formTemplateService.CreateAsync(tenantSlug, model, user);

            return id;
        }

        [Returns404]
        [HttpPost("defaults")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<CreateResult> CreateDefaults(string tenantSlug)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _formTemplateService.CreateDefaultsAsync(tenantSlug, user);

            return Created();
        }

        [Returns404]
        [HttpPut("{templateId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Edit(string tenantSlug, int templateId, FormTemplateModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _formTemplateService.EditAsync(tenantSlug, templateId, model, user);

            return NoContent();
        }

        [HttpGet("")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<List<FormTemplateResult>> List(string tenantSlug)
        {
            var user = await GetCurrentUserAsync(_userService);

            var formTemplates = await _formTemplateService.GetListAsync(tenantSlug, user);

            return formTemplates.Select(item => new FormTemplateResult(item)).ToList();
        }

        [Authorize]
        [Returns404]
        [HttpGet("{templateId}")]
        public async Task<FormTemplateResult> Get(int templateId)
        {
            var user = await GetCurrentUserAsync(_userService);

            var formTemplate = await _formTemplateService.GetAsync(templateId, user);

            return new FormTemplateResult(formTemplate);
        }

        [Returns404]
        [HttpDelete("{templateId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Delete(int templateId)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _formTemplateService.DeleteAsync(templateId, user);

            return NoContent();
        }
    }
}