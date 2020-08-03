using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public interface IClientService
    {
        Task CreateAsync(string tenantSlug, ClientModel model, User user);

        Task EditAsync(int clientId, ClientModel model, User user);

        Task<List<Client>> GetListAsync(string tenantSlug, User user);

        Task ArchiveAsync(int clientId, User user);

        Task SetPmuAsync(int clientId, ClientPmuModel model, User user);

        Task<Client> GetAsync(int clientId, User user);
    }
}