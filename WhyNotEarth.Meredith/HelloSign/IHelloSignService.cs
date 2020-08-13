using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.HelloSign
{
    public interface IHelloSignService
    {
        string? GetDownloadableSignaturesAsync(string json);

        byte[] DownloadSignature(string signatureRequestId);

        internal Task<string> GetSignatureRequestAsync(Client client, User user, Public.Tenant tenant);
    }
}