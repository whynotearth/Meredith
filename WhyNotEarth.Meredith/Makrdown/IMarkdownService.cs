using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.Makrdown
{
    public interface IMarkdownService
    {
        public string ToHtml(string markdown);

        public Task<byte[]> ToPdfAsync(string markdown);
    }
}