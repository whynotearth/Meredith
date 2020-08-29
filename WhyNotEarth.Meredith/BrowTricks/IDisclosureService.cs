using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public interface IDisclosureService
    {
        Task CreateAsync(string tenantSlug, DisclosureModel model, User user);

        Task<List<Disclosure>> ListAsync(string tenantSlug, User user);
    }
}