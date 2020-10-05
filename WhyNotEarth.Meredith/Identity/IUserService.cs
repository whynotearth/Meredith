using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WhyNotEarth.Meredith.Identity.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity
{
    public interface IUserService
    {
        Task<User> GetUserAsync(ClaimsPrincipal principal);

        Task<User?> GetUserAsync(string email);

        Task<List<User>> ListAsync(Public.Tenant tenant);

        Task<UserCreateResult> CreateAsync(RegisterModel model);

        Task<bool> IsExternalAccountConnected(User user);

        User Map(User user, ExternalLoginInfo externalLoginInfo);

        Task<IdentityResult> UpdateUserAsync(User user, ProfileModel model);

        Task<string> GenerateJwtTokenAsync(User user);

        Task SendConfirmPhoneNumberTokenAsync(User user, ConfirmPhoneNumberTokenModel model);

        Task<IdentityResult> ConfirmPhoneNumberAsync(User user, ConfirmPhoneNumberModel model);

        Task SendForgotPasswordAsync(ForgotPasswordModel model);

        Task<IdentityResult> ForgotPasswordResetAsync(ForgotPasswordResetModel model);
    }
}