using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.HelloSign
{
    public interface IPmuPdfService
    {
        public Task<byte[]> GetPngAsync(Public.Tenant tenant);

        public string GetHtml(FormSignature formSignature);
    }
}