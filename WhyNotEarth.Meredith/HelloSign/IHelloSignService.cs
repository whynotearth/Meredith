using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.HelloSign
{
    public interface IHelloSignService
    {
        internal Task<string> GetSignatureRequestAsync(int clientId);

        Task ProcessEventsAsync(string json);
    }
}