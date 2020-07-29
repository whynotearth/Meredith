using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public interface IPmuService
    {
        Task<string> SetAsync(int clientId, ClientPmuModel model, User user);

        Task SetSignedAsync(int clientId, User user);

        Task SendConsentNotificationAsync(int clientId, User user, string callbackUrl);

        void ProcessHelloSignCallback(string json);
    }
}