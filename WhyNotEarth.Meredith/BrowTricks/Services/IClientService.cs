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

        Task<(List<Client>, int recordCount)> GetListAsync(string tenantSlug, User user, int page);

        Task ArchiveAsync(int clientId, User user);

        Task<Client> GetAsync(int clientId, User user);

        Task ValidateOwnerOrSelfAsync(int clientId, User user);

        Task<Public.Tenant> ValidateOwnerOrClientAsync(int tenantId, User user);

        Task<Client> ValidateOwnerAsync(int clientId, User user);
    }
}