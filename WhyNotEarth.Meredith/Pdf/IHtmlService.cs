using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.Pdf
{
    public interface IHtmlService
    {
        Task<byte[]> ToPdfAsync(string html);

        Task<byte[]> ToPngAsync(string html);
    }
}