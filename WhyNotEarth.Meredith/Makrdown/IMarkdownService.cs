using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.Makrdown
{
    public interface IMarkdownService
    {
        string ToHtml(string markdown);

        Task<byte[]> ToPdfAsync(string markdown);
    }
}