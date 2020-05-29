using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Models;

namespace WhyNotEarth.Meredith.Identity
{
    public class UserService
    {
        private readonly UserManager _userManager;

        public UserService(UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserCreateResult> CreateAsync(RegisterModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Name = model.Email,
                    Address = model.Address,
                    GoogleLocation = model.GoogleLocation
                };

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
    }

    public class UserCreateResult
    {
        public IdentityResult IdentityResult { get; }

        public User? User { get; }

        public UserCreateResult(IdentityResult identityResult, User? user)
        {
            IdentityResult = identityResult;

            if (identityResult.Succeeded)
            {
                User = user;
            }
        }
    }
}