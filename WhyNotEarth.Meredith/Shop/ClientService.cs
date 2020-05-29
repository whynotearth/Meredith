using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Models;

namespace WhyNotEarth.Meredith.Shop
{
    public class ClientService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly UserManager _userManager;
        private readonly UserService _userService;

        public ClientService(MeredithDbContext meredithDbContext, UserManager userManager, UserService userService)
        {
            _dbContext = meredithDbContext;
            _userManager = userManager;
            _userService = userService;
        }

        public Task<List<Client>> ListAsync(Data.Entity.Models.Modules.Shop.Tenant tenant)
        {
            return _dbContext.Clients
                .Include(item => item.User)
                .Where(item => item.TenantId == tenant.Id)
                .ToListAsync();
        }

        public async Task CreateAsync(ClientModel model, Data.Entity.Models.Modules.Shop.Tenant tenant)
        {
            var user = await GetUserAsync(model.User);

            var client = new Client
            {
                UserId = user.Id,
                TenantId = tenant.Id
            };

            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<User> GetUserAsync(RegisterModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                return user;
            }

            var userCreateResult = await _userService.CreateAsync(model);

            if (!userCreateResult.IdentityResult.Succeeded)
            {
                throw new InvalidActionException(userCreateResult.IdentityResult.Errors);
            }

            return userCreateResult.User!;
        }
    }
}