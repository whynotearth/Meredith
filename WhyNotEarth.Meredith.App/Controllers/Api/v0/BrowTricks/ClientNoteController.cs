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
    [Route("api/v0/browtricks/tenants/{tenantSlug}/clients/{clientId}/notes")]
    public class ClientNoteController : BaseController
    {
        private readonly ClientNoteService _clientNoteService;
        private readonly IUserService _userService;

        public ClientNoteController(ClientNoteService clientNoteService, IUserService userService)
        {
            _clientNoteService = clientNoteService;
            _userService = userService;
        }

        [Returns204]
        [Returns404]
        [HttpPost("")]
        public async Task<NoContentResult> SaveNote(int clientId, ClientNoteModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientNoteService.SaveAsync(clientId, model, user);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpDelete("{noteId}")]
        public async Task<NoContentResult> DeleteNote(int clientId, int noteId)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientNoteService.DeleteAsync(clientId, noteId, user);

            return NoContent();
        }

        [Returns200]
        [Returns404]
        [HttpGet("")]
        public async Task<ActionResult<List<ClientNoteResult>>> ListNote(int clientId)
        {
            var user = await _userService.GetUserAsync(User);

            var notes = await _clientNoteService.ListAsync(clientId, user);

            return notes?.Select(item => new ClientNoteResult(item)).ToList() ?? new List<ClientNoteResult>();
        }
    }
}