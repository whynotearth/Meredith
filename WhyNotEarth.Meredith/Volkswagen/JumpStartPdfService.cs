using System;
using System.IO;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.GoogleCloud;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartPdfService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly GoogleStorageService _googleStorageService;
        private readonly JumpStartEmailTemplateService _jumpStartEmailTemplateService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public JumpStartPdfService(MeredithDbContext dbContext, GoogleStorageService googleStorageService,
            JumpStartEmailTemplateService jumpStartEmailTemplateService, IBackgroundJobClient backgroundJobClient)
        {
            _dbContext = dbContext;
            _googleStorageService = googleStorageService;
            _jumpStartEmailTemplateService = jumpStartEmailTemplateService;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task CreatePdfAsync(int jumpStartId)
        {
            var jumpStart =
                await _dbContext.JumpStarts.FirstOrDefaultAsync(item => item.Id == jumpStartId && item.HasPdf == false);
            
            if (jumpStart is null)
            {
                // We already created pdf for this one
                return;
            }

            var pdfStream = await BuildPdfAsync();

            await UploadPdfAsync(jumpStart, pdfStream);

            jumpStart.HasPdf = true;
            _dbContext.Update(jumpStart);

            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Schedule<JumpStartEmailService>(service =>
                service.SendAsync(jumpStart.Id), DateTime.UtcNow - jumpStart.DateTime);
        }

        public Task<string> CreatePdfUrlAsync(JumpStart jumpStart)
        {
            return _googleStorageService.CreateSignedUrlAsync(GetName(jumpStart), 24);
        }

        private async Task<Stream> BuildPdfAsync()
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            await using var page = await browser.NewPageAsync();

            var emailTemplate = _jumpStartEmailTemplateService.GetEmailTemplate();

            await page.SetContentAsync(emailTemplate);

            return await page.PdfStreamAsync(new PdfOptions
            {
                PrintBackground = true
            });
        }

        private async Task UploadPdfAsync(JumpStart jumpStart, Stream pdfStream)
        {
            await _googleStorageService.UploadPdfAsync(GetName(jumpStart), pdfStream);
        }

        private string GetName(JumpStart jumpStart)
        {
            return $"volkswagen_pdf_{jumpStart.DateTime.Date:yyyy_MM_dd}.pdf";
        }
    }
}