using System.Threading.Tasks;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks;

namespace WhyNotEarth.Meredith.HelloSign
{
    public interface IHelloSignService
    {
        string? GetDownloadableSignaturesAsync(string json);

        byte[] DownloadSignature(string signatureRequestId);

        internal Task<string> GetSignatureRequestAsync(Client client, User user, Data.Entity.Models.Tenant tenant);
    }
}