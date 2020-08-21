using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.Pdf
{
    public interface IHtmlService
    {
        public Task<byte[]> ToPdfAsync(string html);

        public Task<byte[]> ToPngAsync(string html);
    }
}