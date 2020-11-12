using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    public class ClientNoteService
    {
        private readonly IDbContext _dbContext;
        private readonly TenantService _tenantService;

        public ClientNoteService(IDbContext dbContext, TenantService tenantService)
        {
            _dbContext = dbContext;
            _tenantService = tenantService;
        }

        public async Task CreateAsync(int clientId, ClientNoteModel model, User user)
        {
            await ValidateAsync(user, clientId);

            var note = Map(new ClientNote(), model, clientId);

            note.CreatedAt = DateTime.UtcNow;

            _dbContext.ClientsNotes.Add(note);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int clientId, int noteId, User user)
        {
            var note = await GetAsync(clientId, noteId, user);

            _dbContext.ClientsNotes.Remove(note);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ClientNote>> ListAsync(int clientId, User user)
        {
            var client = await ValidateAsync(user, clientId);

            return client.Notes;
        }

        public async Task EditAsync(int clientId, int noteId, ClientNoteModel model, User user)
        {
            var note = await GetAsync(clientId, noteId, user);

            note = Map(note, model, clientId);

            _dbContext.ClientsNotes.Update(note);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ClientNote> GetAsync(int clientId, int noteId, User user)
        {
            var note = await _dbContext.ClientsNotes
                .Include(item => item.Client)
                .FirstOrDefaultAsync(item => item.Id == noteId && item.ClientId == clientId);

            if (note is null)
            {
                throw new RecordNotFoundException($"note {noteId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, note.Client.TenantId);

            return note;
        }

        private ClientNote Map(ClientNote note, ClientNoteModel model, int clientId)
        {
            note.Note = model.Note;
            note.ClientId = clientId;

            return note;
        }

        private async Task<Client> ValidateAsync(User user, int clientId)
        {
            var client = await _dbContext.Clients
                .Include(item => item.Notes)
                .FirstOrDefaultAsync(item => item.Id == clientId);

            if (client is null)
            {
                throw new RecordNotFoundException($"client {clientId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, client.TenantId);

            return client;
        }
    }
}