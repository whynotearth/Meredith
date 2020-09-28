using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public interface IFormSignatureFileService
    {
        public Task<byte[]> GetPngAsync(FormTemplate formTemplate);

        public Task<byte[]> GetPngAsync(FormSignature formTemplate);

        public string GetHtml(FormSignature formSignature);
    }
}