using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Cloudinary;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.HelloSign;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Models;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks
{
    internal class ClientService : IClientService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IHelloSignService _helloSignService;
        private readonly MeredithDbContext _dbContext;
        private readonly TenantService _tenantService;
        private readonly IUserService _userService;

        public ClientService(IUserService userService, MeredithDbContext dbContext, TenantService tenantService,
            ICloudinaryService cloudinaryService, IHelloSignService helloSignService)
        {
            _userService = userService;
            _dbContext = dbContext;
            _tenantService = tenantService;
            _cloudinaryService = cloudinaryService;
            _helloSignService = helloSignService;
        }

        public async Task CreateAsync(string tenantSlug, ClientModel model, User user)
        {
            var tenant = await _tenantService.CheckPermissionAsync(user, tenantSlug);

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
            var tenant = await _tenantService.CheckPermissionAsync(user, tenantSlug);

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

        public async Task<string> SetPmuAsync(int clientId, ClientPmuModel model, User user)
        {
            var client = await GetAsync(clientId, user);

            if (client.PmuStatus != PmuStatusType.Incomplete)
            {
                throw new InvalidActionException("This client is already signed their PMU form");
            }

            var disclosures = model.Disclosures.Select(item => new Disclosure
            {
                ClientId = clientId,
                Value = item
            });

            _dbContext.Disclosures.AddRange(disclosures);
            await _dbContext.SaveChangesAsync();

            return await _helloSignService.GetSignatureRequestAsync(clientId);
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

            await _tenantService.CheckPermissionAsync(user, client.TenantId);

            return client;
        }

        public async Task SetPmuSignedAsync(int clientId, User user)
        {
            var client = await GetAsync(clientId, user);

            client.PmuStatus = PmuStatusType.Saving;

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

        private async Task<Client> MapClientAsync(Client client, ClientModel model,
            Data.Entity.Models.Tenant? tenant = null)
        {
            if (client.User is null)
            {
                client.User = await GetOrCreateUserAsync(model);
            }
            else
            {
                client.User.Email = model.Email;
                client.User.FirstName = model.FirstName;
                client.User.LastName = model.LastName;
                client.User.PhoneNumber = model.PhoneNumber;

                await _userService.UpdateUserAsync(client.User);
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