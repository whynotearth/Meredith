using System.Threading.Tasks;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public interface IPmuService
    {
        Task<byte[]> GetPdfAsync(int clientId, User user);

        Task SignAsync(int clientId, User user);

        Task SendConsentNotificationAsync(int clientId, User user, string callbackUrl);
    }
}