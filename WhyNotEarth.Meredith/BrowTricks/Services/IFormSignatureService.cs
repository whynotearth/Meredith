using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    public interface IFormSignatureService
    {
        Task<FormSignature?> GetAsync(Client client, FormTemplateType type);

        Task<Dictionary<Client, FormSignature?>> GetAsync(List<Client> clients, FormTemplateType type);
    }
}