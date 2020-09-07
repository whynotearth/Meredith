using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    public interface IPmuService
    {
        Task<byte[]> GetPngAsync(string tenantSlug, User user);

        Task<byte[]> GetPngAsync(int clientId, User user);

        Task SignAsync(int clientId, PmuSignModel model, User user);

        Task SendConsentNotificationAsync(int clientId, User user, string callbackUrl);
    }
}