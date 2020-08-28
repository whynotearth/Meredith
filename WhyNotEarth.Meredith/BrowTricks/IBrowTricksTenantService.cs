using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public interface IBrowTricksTenantService
    {
        Task<List<ClientImage>> GetAllImages(string tenantSlug, User user);

        Task<List<ClientVideo>> GetAllVideos(string tenantSlug, User user);
    }
}