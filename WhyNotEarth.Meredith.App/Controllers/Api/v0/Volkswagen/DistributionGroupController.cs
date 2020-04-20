using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen.DistributionGroup;
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

        [HttpGet("")]
        public async Task<ActionResult<List<string>>> Create()
        {
            var result = await _recipientService.GetDistinctDistributionGroups();
            
            return Ok(result);
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DistributionGroupStatResult>> Stats()
        {
            var stats = await _recipientService.GetDistributionGroupStats();

            return Ok(stats.Select(item => new DistributionGroupStatResult(item)));
        }
    }
}