using System.Threading.Tasks;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.HelloSign
{
    public interface IHelloSignService
    {
        Task<string> GetSignatureRequestAsync(string tenantSlug, User user);

        Task ProcessEventsAsync(string json);
    }
}