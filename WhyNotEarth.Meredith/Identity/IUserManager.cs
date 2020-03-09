using System.Security.Claims;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.Identity
{
    public interface IUserManager
    {
        public Task<User> GetUserAsync(ClaimsPrincipal principal);
    }
}