using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    public interface IBrowTricksService
    {
        Task<List<ClientImage>> GetAllImages(User user);

        Task<List<ClientImage>> GetAllImages(string tenantSlug, User user);

        Task<List<ClientVideo>> GetAllVideos(User user);

        Task<List<ClientVideo>> GetAllVideos(string tenantSlug, User user);
    }
}