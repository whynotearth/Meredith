using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/distributiongroups")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class DistributionGroupController : ControllerBase
    {
        private readonly RecipientService _recipientService;

        public DistributionGroupController(RecipientService recipientService)
        {
            _recipientService = recipientService;
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<string>>> List()
        {
            var result = await _recipientService.GetDistributionGroupsAsync();
            
            return Ok(result);
        }

        [Returns200]
        [HttpGet("stats")]
        public async Task<ActionResult<DistributionGroupStatResult>> Stats()
        {
            var stats = await _recipientService.GetDistributionGroupStatsAsync();

            return Ok(stats.Select(item => new DistributionGroupStatResult(item)));
        }

        [Returns204]
        [HttpPut("")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            await _recipientService.ImportAsync(file.OpenReadStream());

            return NoContent();
        }

        [Returns200]
        [HttpGet("{distributionGroupName}/recipients")]
        public async Task<ActionResult<List<RecipientResult>>> ListRecipients(string distributionGroupName)
        {
            var recipients = await _recipientService.GetRecipientsAsync(distributionGroupName);

            var result = recipients.Select(item => new RecipientResult(item)).ToList();

            return Ok(result);
        }

        [Returns204]
        [HttpPost("{distributionGroupName}/recipients")]
        public async Task<NoContentResult> Add(string distributionGroupName, RecipientModel model)
        {
            await _recipientService.AddAsync(distributionGroupName, model.Email!);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpPut("{distributionGroupName}/recipients/{recipientId}")]
        public async Task<NoContentResult> Edit(int recipientId, RecipientModel model)
        {
            await _recipientService.EditAsync(recipientId, model.Email!);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpDelete("{distributionGroupName}/recipients/{recipientId}")]
        public async Task<NoContentResult> Delete(int recipientId)
        {
            await _recipientService.DeleteAsync(recipientId);

            return NoContent();
        }
    }
}