using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    public interface IFormSignatureService
    {
        Task<List<string>> GetSignatureUrlsAsync(Client client);
    }
}