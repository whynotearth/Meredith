using System.Threading.Tasks;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public interface IFormSignatureFileService
    {
        Task<byte[]> GetPngAsync(FormTemplate formTemplate);

        Task<byte[]> GetPngAsync(FormSignature formTemplate, User user);

        string GetHtml(FormSignature formSignature);
    }
}