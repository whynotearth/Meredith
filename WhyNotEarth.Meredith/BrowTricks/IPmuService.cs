using System.Threading.Tasks;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public interface IPmuService
    {
        Task<byte[]> GetPngAsync(string tenantSlug, User user);

        Task<byte[]> GetPngAsync(int clientId, User user);

        Task SignAsync(int clientId, User user);

        Task SendConsentNotificationAsync(int clientId, User user, string callbackUrl);
    }
}