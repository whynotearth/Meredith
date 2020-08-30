using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.HelloSign
{
    public interface IPmuPdfService
    {
        public Task<byte[]> GetPdfAsync(Public.Tenant tenant, List<Disclosure> disclosures);

        public Task<byte[]> GetPngAsync(Public.Tenant tenant, List<Disclosure> disclosures);
    }
}