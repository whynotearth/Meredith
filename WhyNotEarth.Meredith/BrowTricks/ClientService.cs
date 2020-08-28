using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Cloudinary;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Models;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks
{
    internal class ClientService : IClientService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IDbContext _dbContext;
        private readonly TenantService _tenantService;
        private readonly IUserService _userService;

        public ClientService(IUserService userService, IDbContext dbContext, TenantService tenantService,
            ICloudinaryService cloudinaryService)
        {
            _userService = userService;
            _dbContext = dbContext;
            _tenantService = tenantService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task CreateAsync(string tenantSlug, ClientModel model, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            var client = await MapClientAsync(new Client(), model, tenant);
            client.PmuStatus = PmuStatusType.Incomplete;

            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditAsync(int clientId, ClientModel model, User user)
        {
            var client = await GetAsync(clientId, user);

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
            var client = await GetAsync(clientId, user);

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
                .FirstOrDefaultAsync(item => item.Id == clientId);

            if (client is null)
            {
                throw new RecordNotFoundException($"client {clientId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, client.TenantId);

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
            if (client.User is null)
            {
                var user = await GetOrCreateUserAsync(model);
                client.UserId = user.Id;
            }
            else
            {
                await _userService.UpdateUserAsync(client.User.Id, model.Email, model.FirstName, model.LastName,
                    model.PhoneNumber);
            }

            if (client.Tenant is null)
            {
                client.Tenant = tenant!;
            }

            client.NotificationType = model.NotificationTypes.ToFlag();
            client.Images = await _cloudinaryService.GetUpdatedValueAsync(client.Images, model.Images);
            client.Videos = await _cloudinaryService.GetUpdatedValueAsync(client.Videos, model.Videos);

            return client;
        }
    }
}