using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.HelloSign
{
    public interface IHelloSignService
    {
        internal Task<string> GetSignatureRequestAsync(int clientId);

        string? GetDownloadableSignaturesAsync(string json);

        byte[] DownloadSignature(string signatureRequestId);
    }
}