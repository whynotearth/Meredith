using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/recipients")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class RecipientController : ControllerBase
    {
        private readonly RecipientService _recipientService;

        public RecipientController(RecipientService recipientService)
        {
            _recipientService = recipientService;
        }

        [Returns204]
        [HttpPut("")]
        public async Task<IActionResult> Create(IFormFile file)
        {
            await _recipientService.Import(file.OpenReadStream());

            return NoContent();
        }
    }
}