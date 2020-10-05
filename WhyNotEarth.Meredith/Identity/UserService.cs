using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Identity.Models;
using WhyNotEarth.Meredith.Identity.Notifications;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity
{
    internal class UserService : IUserService
    {
        private readonly CompanyService _companyService;
        private readonly IDbContext _dbContext;
        private readonly JwtOptions _jwtOptions;
        private readonly UserManager _userManager;
        private readonly IUserNotificationService _userNotificationService;

        public UserService(UserManager userManager, IDbContext dbContext, IOptions<JwtOptions> jwtOptions,
            CompanyService companyService, IUserNotificationService userNotificationService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _companyService = companyService;
            _userNotificationService = userNotificationService;
            _jwtOptions = jwtOptions.Value;
        }

        public Task<User> GetUserAsync(ClaimsPrincipal principal)
        {
            return _userManager.GetUserAsync(principal);
        }

        public Task<User?> GetUserAsync(string email)
        {
            return _userManager.FindByEmailAsync(email)!;
        }

        public Task<List<User>> ListAsync(Public.Tenant tenant)
        {
            return _dbContext.Users
                .Where(item => item.TenantId == tenant.Id)
                .ToListAsync();
        }

        public async Task<UserCreateResult> CreateAsync(RegisterModel model)
        {
            if (model.Email is null)
            {
                var newUser = await MapUserAsync(model);

                return await CreateAsync(newUser, model.Password);
            }

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

        public async Task<IdentityResult> UpdateUserAsync(User user, ProfileModel model)
        {
            var identityResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);

            if (!identityResult.Succeeded)
            {
                return identityResult;
            }

            var shouldUpdateUsername = user.UserName == user.Email;
            identityResult = await _userManager.SetEmailAsync(user, model.Email);

            if (!identityResult.Succeeded)
            {
                return identityResult;
            }

            if (shouldUpdateUsername)
            {
                identityResult = await _userManager.SetUserNameAsync(user, model.Email);
            }

            if (!identityResult.Succeeded)
            {
                return identityResult;
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.GoogleLocation = model.GoogleLocation;

            return await _userManager.UpdateAsync(user);
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            if (!_jwtOptions.IsValid())
            {
                throw new Exception("Missing JWT configurations.");
            }

            var claims = new List<Claim>
            {
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

        public async Task SendConfirmPhoneNumberTokenAsync(User user, ConfirmPhoneNumberTokenModel model)
        {
            var company = await _companyService.GetAsync(model.CompanySlug);
            var tenant = await GetTenantAsync(model.TenantSlug);

            if (user.PhoneNumberConfirmed)
            {
                throw new InvalidActionException("User already has a confirmed phone number");
            }

            if (user.PhoneNumber is null && model.PhoneNumber is null)
            {
                throw new InvalidActionException("Please provide a phone number");
            }

            if (model.PhoneNumber != null)
            {
                user.PhoneNumber = model.PhoneNumber;
                var identityResult = await _userManager.UpdateAsync(user);

                if (!identityResult.Succeeded)
                {
                    throw new InvalidActionException(identityResult.Errors);
                }
            }

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

            await _userNotificationService.NotifyAsync(user, NotificationType.Sms,
                new ConfirmPhoneNumberNotification(company, tenant, token));
        }

        public Task<IdentityResult> ConfirmPhoneNumberAsync(User user, ConfirmPhoneNumberModel model)
        {
            return _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, model.Token);
        }

        public async Task SendForgotPasswordAsync(ForgotPasswordModel model)
        {
            var company = await _companyService.GetAsync(model.CompanySlug);

            User user;
            string unique;
            string uniqueValue;
            if (model.Email != null)
            {
                user = await _userManager.FindByEmailAsync(model.Email);
                unique = "email";
                uniqueValue = model.Email;
            }
            else
            {
                user = await _userManager.FindByNameAsync(model.UserName);
                unique = "username";
                uniqueValue = model.UserName!;
            }

            if (user is null)
            {
                // Don't reveal that the user does not exist
                return;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var uriBuilder = new UriBuilder(model.ReturnUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[unique] = uniqueValue;
            query["token"] = token;
            uriBuilder.Query = query.ToString();
            var callbackUrl = uriBuilder.ToString();

            await _userNotificationService.NotifyAsync(user, new ForgotPasswordNotification(company, callbackUrl)
            {
                Subject = "Reset Password"
            });
        }

        public async Task ForgotPasswordResetAsync(ForgotPasswordResetModel model)
        {
            User user;
            if (model.Email != null)
            {
                user = await _userManager.FindByEmailAsync(model.Email);
            }
            else
            {
                user = await _userManager.FindByNameAsync(model.UserName);
            }

            if (user is null)
            {
                // Don't reveal that the user does not exist
                return;
            }

            await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
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
                UserName = model.UserName ?? model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                GoogleLocation = model.GoogleLocation,
                TenantId = tenantId
            };
        }

        public async Task<Public.Tenant?> GetTenantAsync(string? tenantSlug)
        {
            if (tenantSlug is null)
            {
                return null;
            }

            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(item => item.Slug == tenantSlug);

            if (tenant is null)
            {
                throw new RecordNotFoundException($"Tenant {tenantSlug} not found");
            }

            return tenant;
        }
    }
}