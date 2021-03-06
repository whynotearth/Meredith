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

        [Returns404]
        [HttpPost("")]
        public async Task<NoContentResult> Create(int clientId, ClientNoteModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientNoteService.CreateAsync(clientId, model, user);

            return NoContent();
        }

        [Returns404]
        [HttpPut("{noteId}")]
        public async Task<NoContentResult> Create(int clientId, int noteId, ClientNoteModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientNoteService.EditAsync(clientId, noteId, model, user);

            return NoContent();
        }

        [Returns404]
        [HttpDelete("{noteId}")]
        public async Task<NoContentResult> Delete(int clientId, int noteId)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientNoteService.DeleteAsync(clientId, noteId, user);

            return NoContent();
        }

        [Returns404]
        [HttpGet("{noteId}")]
        public async Task<ClientNoteResult> Get(int clientId, int noteId)
        {
            var user = await _userService.GetUserAsync(User);

            var note = await _clientNoteService.GetAsync(clientId, noteId, user);

            return new ClientNoteResult(note);
        }

        [Returns404]
        [HttpGet("")]
        public async Task<List<ClientNoteResult>> List(int clientId)
        {
            var user = await _userService.GetUserAsync(User);

            var notes = await _clientNoteService.ListAsync(clientId, user);

            return notes?.Select(item => new ClientNoteResult(item, 500)).ToList() ?? new List<ClientNoteResult>();
        }
    }
}