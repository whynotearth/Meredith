using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class PuppeteerService
    {
        private readonly JumpStartEmailTemplateService _jumpStartEmailTemplateService;

        public PuppeteerService(JumpStartEmailTemplateService jumpStartEmailTemplateService)
        {
            _jumpStartEmailTemplateService = jumpStartEmailTemplateService;
        }

        public async Task<Stream> BuildPdfAsync(DateTime date, List<Post> posts)
        {
            var browser = await GetBrowser();

            await using var page = await browser.NewPageAsync();

            var emailTemplate = _jumpStartEmailTemplateService.GetEmailHtml(date, posts);

            await page.SetContentAsync(emailTemplate);

            return await page.PdfStreamAsync(new PdfOptions
            {
                PrintBackground = true
            });
        }

        public async Task<Stream> BuildScreenshotAsync(List<Post> posts)
        {
            var browser = await GetBrowser();

            await using var page = await browser.NewPageAsync();

            var emailTemplate = _jumpStartEmailTemplateService.GetEmailHtml(DateTime.UtcNow, posts);

            await page.SetContentAsync(emailTemplate);

            return await page.ScreenshotStreamAsync();
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