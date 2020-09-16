using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    public interface IFormSignatureService
    {
        Task<Dictionary<int, string?>> GetSignatureUrlsAsync(Client client);
    }
}