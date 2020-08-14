using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WhyNotEarth.Meredith.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity
{
    public interface IUserService
    {
        Task<User> GetUserAsync(ClaimsPrincipal principal);

        Task<User> GetUserAsync(string email);

        Task<List<User>> ListAsync(Public.Tenant tenant);

        Task<UserCreateResult> CreateAsync(RegisterModel model);

        Task<bool> IsExternalAccountConnected(User user);

        User Map(User user, ExternalLoginInfo externalLoginInfo);

        Task UpdateUserAsync(int userId, string email, string? firstName, string? lastName, string? phoneNumber);

        Task<string> GenerateJwtTokenAsync(User user);
    }
}