using System.Threading.Tasks;
using PuppeteerSharp;

namespace WhyNotEarth.Meredith.Pdf
{
    internal class HtmlService : IHtmlService
    {
        public async Task<byte[]> ToPdfAsync(string html)
        {
            await using var browser = await GetBrowser();

            await using var page = await browser.NewPageAsync();

            await page.SetContentAsync(html);

            var pdfData = await page.PdfDataAsync(new PdfOptions
            {
                PrintBackground = true
            });

            return pdfData;
        }

        public async Task<byte[]> ToPngAsync(string html)
        {
            await using var browser = await GetBrowser();

            await using var page = await browser.NewPageAsync();

            await page.SetContentAsync(html);

            var screenShotData = await page.ScreenshotDataAsync(new ScreenshotOptions
            {
                FullPage = true
            });

            return screenShotData;
        }

        private async Task<Browser> GetBrowser()
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            return await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
        }
    }
}