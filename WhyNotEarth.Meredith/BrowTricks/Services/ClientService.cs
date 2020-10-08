using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Identity.Models;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    internal class ClientService : IClientService
    {
        private readonly IDbContext _dbContext;
        private readonly TenantService _tenantService;
        private readonly IUserService _userService;

        public ClientService(IUserService userService, IDbContext dbContext, TenantService tenantService)
        {
            _userService = userService;
            _dbContext = dbContext;
            _tenantService = tenantService;
        }

        public async Task CreateAsync(string tenantSlug, ClientModel model, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            var client = await MapClientAsync(new Client(), model, tenant);

            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditAsync(int clientId, ClientModel model, User user)
        {
            var client = await _dbContext.Clients
                .Include(item => item.User)
                .Include(item => item.Images)
                .Include(item => item.Videos)
                .FirstOrDefaultAsync(item => item.Id == clientId);

            if (client is null)
            {
                throw new RecordNotFoundException($"client {clientId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, client.TenantId);

            client = await MapClientAsync(client, model);

            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Client>> GetListAsync(string tenantSlug, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            return await _dbContext.Clients
                .Include(item => item.User)
                .Where(item => item.TenantId == tenant.Id && item.IsArchived == false)
                .ToListAsync();
        }

        public async Task ArchiveAsync(int clientId, User user)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(item => item.Id == clientId);

            if (client is null)
            {
                throw new RecordNotFoundException($"client {clientId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, client.TenantId);

            client.IsArchived = true;

            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Client> GetAsync(int clientId, User user)
        {
            var client = await _dbContext.Clients
                .Include(item => item.User)
                .Include(item => item.Images)
                .Include(item => item.Videos)
                .Include(item => item.Tenant)
                .FirstOrDefaultAsync(item => item.Id == clientId);

            if (client is null)
            {
                throw new RecordNotFoundException($"client {clientId} not found");
            }

            ValidateOwnerOrSelf(client, user);

            return client;
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

        private async Task<Client> MapClientAsync(Client client, ClientModel model,
            Public.Tenant? tenant = null)
        {
            if (client.User is null!)
            {
                var user = await GetOrCreateUserAsync(model);
                client.UserId = user.Id;
            }
            else
            {
                await _userService.UpdateUserAsync(client.User, new ProfileModel
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                });
            }

            if (client.Tenant is null!)
            {
                client.Tenant = tenant!;
            }

            return client;
        }

        public async Task<Client> ValidateOwnerOrSelfAsync(int clientId, User user)
        {
            var client = await _dbContext.Clients
                .Include(item => item.User)
                .Include(item => item.Tenant)
                .FirstOrDefaultAsync(item => item.Id == clientId);

            if (client is null)
            {
                throw new RecordNotFoundException($"client {clientId} not found");
            }

            return ValidateOwnerOrSelf(client, user);
        }

        public async Task<Public.Tenant> ValidateOwnerOrClientAsync(int tenantId, User user)
        {
            var tenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(item => item.Id == tenantId && item.OwnerId == user.Id);

            if (tenant != null)
            {
                // It's the owner
                return tenant;
            }

            var client = await _dbContext.Clients
                .Include(item => item.Tenant)
                .FirstOrDefaultAsync(item => item.TenantId == tenantId && item.UserId == user.Id);

            if (client != null)
            {
                // It's one of the clients
                return client.Tenant;
            }

            throw new ForbiddenException();
        }

        public async Task<Client> ValidateOwnerAsync(int clientId, User user)
        {
            var client = await _dbContext.Clients
                .Include(item => item.Tenant)
                .FirstOrDefaultAsync(item => item.Id == clientId);

            if (client is null)
            {
                throw new RecordNotFoundException($"client {clientId} not found");
            }

            if (client.Tenant.OwnerId != user.Id)
            {
                throw new ForbiddenException();
            }

            return client;
        }

        private Client ValidateOwnerOrSelf(Client client, User user)
        {
            if (client.UserId == user.Id)
            {
                // It's the user itself
                return client;
            }

            if (client.Tenant.OwnerId == user.Id)
            {
                // It's the owner
                return client;
            }

            throw new ForbiddenException();
        }
    }
}