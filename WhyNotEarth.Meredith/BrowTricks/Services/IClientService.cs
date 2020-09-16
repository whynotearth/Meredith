using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    public interface IClientService
    {
        Task CreateAsync(string tenantSlug, ClientModel model, User user);

        Task EditAsync(int clientId, ClientModel model, User user);

        Task<List<Client>> GetListAsync(string tenantSlug, User user);

        Task ArchiveAsync(int clientId, User user);

        Task<Client> GetAsync(int clientId, User user);

        Task<Client> ValidateOwnerOrSelf(int clientId, User user);

        Task<Public.Tenant> ValidateOwnerOrClient(int tenantId, User user);
    }
}