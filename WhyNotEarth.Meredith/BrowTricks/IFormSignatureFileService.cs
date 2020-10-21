using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public interface IFormSignatureFileService
    {
        Task<byte[]> GetPngAsync(FormTemplate formTemplate);

        Task<byte[]> GetPdfAsync(FormTemplate formTemplate);

        Task<byte[]> GetPngAsync(FormSignature formTemplate, string fullName);

        string GetHtml(FormSignature formSignature);
    }
}