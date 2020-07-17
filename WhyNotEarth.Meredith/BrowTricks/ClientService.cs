using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Models;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public class ClientService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly TenantService _tenantService;
        private readonly IUserService _userService;

        public ClientService(IUserService userService, MeredithDbContext dbContext, TenantService tenantService)
        {
            _userService = userService;
            _dbContext = dbContext;
            _tenantService = tenantService;
        }

        public async Task CreateAsync(string tenantSlug, ClientModel model, User user)
        {
            var tenant = await ValidateAsync(user, tenantSlug);

            var client = await MapAsync(new Client(), model, tenant);

            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditAsync(int clientId, ClientModel model, User user)
        {
            var client = await ValidateAsync(user, clientId);

            client = await MapAsync(client, model);

            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Client>> GetListAsync(string tenantSlug, User user)
        {
            var tenant = await _tenantService.CheckPermissionAsync(user, tenantSlug);

            return await _dbContext.Clients
                .Include(item => item.User)
                .Where(item => item.TenantId == tenant.Id).ToListAsync();
        }

        public async Task ArchiveAsync(int clientId, User user)
        {
            var client = await ValidateAsync(user, clientId);

            client.IsArchived = true;

            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SetPmuAsync(int clientId, ClientPmuModel model, User user)
        {
            var client = await ValidateAsync(user, clientId);

            client = Map(client, model);

            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<User> GetOrCreateUserAsync(ClientModel model)
        {
            var user = await _userService.GetUserAsync(model.Email);

            if (user != null)
            {
                return user;
            }

            var userCreateResult = await _userService.CreateAsync(new RegisterModel
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber
            });

            if (!userCreateResult.IdentityResult.Succeeded)
            {
                throw new InvalidActionException(userCreateResult.IdentityResult.Errors);
            }

            return userCreateResult.User!;
        }

        private async Task<Client> MapAsync(Client client, ClientModel model, Data.Entity.Models.Tenant? tenant = null)
        {
            var user = await GetOrCreateUserAsync(model);

            if (tenant != null)
            {
                client.Tenant = tenant;
            }

            client.User = user;
            client.NotificationType = model.NotificationTypes.ToFlag();

            return client;
        }

        private async Task<Data.Entity.Models.Tenant> ValidateAsync(User user, string tenantSlug)
        {
            return await _tenantService.CheckPermissionAsync(user, tenantSlug);
        }

        private async Task<Client> ValidateAsync(User user, int clientId)
        {
            var client = await GetClientAsync(clientId);

            await _tenantService.CheckPermissionAsync(user, client.TenantId);

            return client;
        }

        private async Task<Client> GetClientAsync(int clientId)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(item => item.Id == clientId);

            if (client is null)
            {
                throw new RecordNotFoundException($"client {clientId} not found");
            }

            return client;
        }

        private Client Map(Client client, ClientPmuModel model)
        {
            client.IsPmuCompleted = true;
            client.Signature = model.Signature;
            client.Initials = model.Initials;
            client.AllowPhoto = model.AllowPhoto;
            client.IsUnderCareOfPhysician = model.IsUnderCareOfPhysician;
            client.Conditions = model.Conditions;
            client.IsTakingBloodThinner = model.IsTakingBloodThinner;
            client.PhysicianName = model.PhysicianName;
            client.PhysicianPhoneNumber = model.PhysicianPhoneNumber;

            return client;
        }
    }
}