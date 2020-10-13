using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    public interface IBrowTricksService
    {
        Task<List<BrowTricksImage>> GetAllImages(User user);

        Task<List<BrowTricksImage>> GetAllImages(string tenantSlug, User user);

        Task<List<BrowTricksVideo>> GetAllVideos(User user);

        Task<List<BrowTricksVideo>> GetAllVideos(string tenantSlug, User user);
    }
}