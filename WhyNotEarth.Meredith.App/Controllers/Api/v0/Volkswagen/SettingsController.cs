using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen.Jobs;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/settings")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class SettingsController : ControllerBase
    {
        private readonly MeredithDbContext _dbContext;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly SettingsService _settingsService;

        public SettingsController(SettingsService settingsService, MeredithDbContext dbContext,
            IRecurringJobManager recurringJobManager)
        {
            _settingsService = settingsService;
            _dbContext = dbContext;
            _recurringJobManager = recurringJobManager;
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<VolkswagenSettingsResult>> Get()
        {
            var settings = await _settingsService.GetValueAsync<VolkswagenSettings>(VolkswagenCompany.Name);

            var result = new VolkswagenSettingsResult(await settings.GetDistributionGroupAsync(_dbContext),
                settings.EnableAutoSend, settings.SendTime);

            return Ok(result);
        }

        [Returns204]
        [HttpPost("")]
        public async Task<NoContentResult> Set(VolkswagenSettingsModel model)
        {
            var settings = new VolkswagenSettings
            {
                DistributionGroups = string.Join(',', model.DistributionGroups),
                EnableAutoSend = model.EnableAutoSend!.Value,
                SendTime = model.SendTime
            };

            await _settingsService.SetValueAsync(VolkswagenCompany.Name, settings);

            return NoContent();
        }
    }
}