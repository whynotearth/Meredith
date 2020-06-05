using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Models;

namespace WhyNotEarth.Meredith.Identity
{
    internal class UserService : IUserService
    {
        private readonly UserManager _userManager;
        private readonly MeredithDbContext _dbContext;

        public UserService(UserManager userManager, MeredithDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public Task<User> GetUserAsync(ClaimsPrincipal principal)
        {
            return _userManager.GetUserAsync(principal);
        }

        public Task<List<User>> ListAsync(Data.Entity.Models.Tenant tenant)
        {
            return _dbContext.Users
                .Where(item => item.TenantId == tenant.Id)
                .ToListAsync();
        }

        public async Task<UserCreateResult> CreateAsync(RegisterModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                user = await MapUserAsync(model);   

                return await CreateAsync(user, model.Password);
            }

            // We only let users that registered without password and
            // also don't have any other provider linked to their account
            // to update or login to their profile without password
            if (user.PasswordHash is null)
            {
                var logins = await _userManager.GetLoginsAsync(user);

                if (!logins.Any())
                {
                    return await UpdateUser(user, model);
                }
            }

            return new UserCreateResult(
                IdentityResult.Failed(new IdentityErrorDescriber().DuplicateUserName(model.Email)), null);
        }

        private async Task<UserCreateResult> CreateAsync(User user, string? password)
        {
            IdentityResult identityResult;

            if (password is null)
            {
                identityResult = await _userManager.CreateAsync(user);
            }
            else
            {
                identityResult = await _userManager.CreateAsync(user, password);
            }

            return new UserCreateResult(identityResult, user);
        }

        private async Task<UserCreateResult> UpdateUser(User user, RegisterModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                user.Name = model.Name;
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                user.PhoneNumber = model.PhoneNumber;
            }

            if (!string.IsNullOrWhiteSpace(model.Address))
            {
                user.Address = model.Address;
            }

            if (!string.IsNullOrWhiteSpace(model.GoogleLocation))
            {
                user.GoogleLocation = model.GoogleLocation;
            }

            var identityResult = await _userManager.UpdateAsync(user);

            return new UserCreateResult(identityResult, user);
        }

        private async Task<User> MapUserAsync(RegisterModel model)
        {
            int? tenantId = null;

            if (!string.IsNullOrWhiteSpace(model.TenantSlug))
            {
                var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(item => item.Slug == model.TenantSlug.ToLower());
                tenantId = tenant?.Id;
            }

            return new User
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Name = model.Email,
                Address = model.Address,
                GoogleLocation = model.GoogleLocation,
                TenantId = tenantId
            };
        }
    }
}