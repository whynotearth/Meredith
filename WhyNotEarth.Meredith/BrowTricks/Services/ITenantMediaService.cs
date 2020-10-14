using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    public interface ITenantMediaService
    {
        Task CreateImageAsync(string tenantSlug, BrowTricksImageModel model, User user);

        Task DeleteImageAsync(int imageId, User user);

        Task CreateVideoAsync(string tenantSlug, BrowTricksVideoModel model, User user);

        Task DeleteVideoAsync(int videoId, User user);
    }
}