using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WhyNotEarth.Meredith.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity
{
    internal class UserService : IUserService
    {
        private readonly IDbContext _dbContext;
        private readonly JwtOptions _jwtOptions;
        private readonly UserManager _userManager;

        public UserService(UserManager userManager, IDbContext dbContext, IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _jwtOptions = jwtOptions.Value;
        }

        public Task<User> GetUserAsync(ClaimsPrincipal principal)
        {
            return _userManager.GetUserAsync(principal);
        }

        public Task<User> GetUserAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public Task<List<User>> ListAsync(Public.Tenant tenant)
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

        public async Task<bool> IsExternalAccountConnected(User user)
        {
            var logins = await _userManager.GetLoginsAsync(user);

            return logins.Any();
        }

        public User Map(User user, ExternalLoginInfo externalLoginInfo)
        {
            user.FirstName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.GivenName);

            if (externalLoginInfo.LoginProvider == GoogleDefaults.AuthenticationScheme)
            {
                user.LastName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name);
            }
            else if (externalLoginInfo.LoginProvider == FacebookDefaults.AuthenticationScheme)
            {
                user.LastName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Surname);
            }

            user.ImageUrl = externalLoginInfo.Principal.FindFirstValue("picture");

            return user;
        }

        public async Task UpdateUserAsync(int userId, string email, string? firstName, string? lastName,
            string? phoneNumber)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            user.Email = email;
            user.UserName = user.Email;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;

            await _userManager.UpdateAsync(user);
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            if (!_jwtOptions.IsValid())
            {
                throw new Exception("Missing JWT configurations.");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_jwtOptions.ExpireDays);

            var token = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
            if (!string.IsNullOrWhiteSpace(model.FirstName))
            {
                user.FirstName = model.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(model.LastName))
            {
                user.LastName = model.LastName;
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
                var tenant =
                    await _dbContext.Tenants.FirstOrDefaultAsync(item => item.Slug == model.TenantSlug.ToLower());
                tenantId = tenant?.Id;
            }

            return new User
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                GoogleLocation = model.GoogleLocation,
                TenantId = tenantId
            };
        }
    }
}