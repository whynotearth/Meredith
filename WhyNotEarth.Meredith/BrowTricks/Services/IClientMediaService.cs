using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    public interface IClientMediaService
    {
        Task CreateImageAsync(ClientImageModel model, User user);

        Task DeleteImageAsync(int imageId, User user);

        Task CreateVideoAsync(ClientVideoModel model, User user);

        Task DeleteVideoAsync(int videoId, User user);
    }
}