using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Volkswagen;

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
        private readonly SettingsService _settingsService;
        private readonly MeredithDbContext _dbContext;

        public SettingsController(SettingsService settingsService, MeredithDbContext dbContext)
        {
            _settingsService = settingsService;
            _dbContext = dbContext;
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<VolkswagenSettingsResult>> Get()
        {
            var settings = await _settingsService.GetValueAsync<VolkswagenSettings>(VolkswagenCompany.Name);

            var result = new VolkswagenSettingsResult(await settings.GetDistributionGroupAsync(_dbContext), settings.SendTime);
            
            return Ok(result);
        }

        [Returns204]
        [HttpPost("")]
        public async Task<NoContentResult> Set(VolkswagenSettingsModel model)
        {
            var settings = new VolkswagenSettings
            {
                DistributionGroups = string.Join(',', model.DistributionGroups),
                SendTime = model.SendTime!.Value
            };

            await _settingsService.SetValueAsync(VolkswagenCompany.Name, settings);
            
            return NoContent();
        }
    }
}