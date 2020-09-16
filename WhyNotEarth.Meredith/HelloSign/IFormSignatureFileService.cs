using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.HelloSign
{
    public interface IFormSignatureFileService
    {
        public Task<byte[]> GetPngAsync(FormTemplate formTemplate);

        public Task<byte[]> GetPngAsync(FormSignature formTemplate);

        public string GetHtml(FormSignature formSignature);
    }
}