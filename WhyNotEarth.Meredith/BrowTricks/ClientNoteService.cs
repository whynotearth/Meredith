using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public class ClientNoteService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly TenantService _tenantService;

        public ClientNoteService(MeredithDbContext dbContext, TenantService tenantService)
        {
            _dbContext = dbContext;
            _tenantService = tenantService;
        }

        public async Task SaveAsync(int clientId, ClientNoteModel model, User user)
        {
            var client = await ValidateAsync(user, clientId);

            client = Map(model, client);

            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int clientId, int noteId, User user)
        {
            var client = await ValidateAsync(user, clientId);

            var clientNote = client.Notes.FirstOrDefault(item => item.Id == noteId);

            if (clientNote is null)
            {
                throw new RecordNotFoundException($"Note {noteId} not found");
            }

            client.Notes = client.Notes.Where(item => item.Id != noteId).ToList();

            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ClientNote>?> ListAsync(int clientId, User user)
        {
            var client = await ValidateAsync(user, clientId);

            return client.Notes;
        }

        private Client Map(ClientNoteModel model, Client client)
        {
            if (model.Id.HasValue)
            {
                var clientNote = client.Notes.FirstOrDefault(item => item.Id == model.Id);

                if (clientNote is null)
                {
                    throw new RecordNotFoundException($"Note {model.Id.Value} not found");
                }

                clientNote.Note = model.Note;
                clientNote.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                client.Notes ??= new List<ClientNote>();

                client.Notes.Add(new ClientNote
                {
                    Note = model.Note,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return client;
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