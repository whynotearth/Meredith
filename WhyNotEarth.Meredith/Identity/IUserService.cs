using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Models;

namespace WhyNotEarth.Meredith.Identity
{
    public interface IUserService
    {
        Task<User> GetUserAsync(ClaimsPrincipal principal);

        Task<User> GetUserAsync(string email);

        Task<List<User>> ListAsync(Data.Entity.Models.Tenant tenant);

        Task<UserCreateResult> CreateAsync(RegisterModel model);

        Task<bool> IsExternalAccountConnected(User user);

        User Map(User user, ExternalLoginInfo externalLoginInfo);
    }
}