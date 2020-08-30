using System.Threading.Tasks;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity
{
    public interface ILoginTokenService
    {
        Task<string> GenerateTokenAsync(User user);

        Task<User?> ValidateTokenAsync(string token);
    }
}