using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.GoogleCloud;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartPdfJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;
        private readonly GoogleStorageService _googleStorageService;
        private readonly PuppeteerService _puppeteerService;

        public JumpStartPdfJob(MeredithDbContext dbContext, GoogleStorageService googleStorageService,
            IBackgroundJobClient backgroundJobClient, PuppeteerService puppeteerService)
        {
            _dbContext = dbContext;
            _googleStorageService = googleStorageService;
            _backgroundJobClient = backgroundJobClient;
            _puppeteerService = puppeteerService;
        }

        public async Task CreatePdfAsync(int jumpStartId)
        {
            var jumpStart = await _dbContext.JumpStarts
                .FirstOrDefaultAsync(item => item.Id == jumpStartId && item.HasPdf == false);

            if (jumpStart is null)
            {
                // We already created pdf for this one
                return;
            }

            var articles = await _dbContext.Articles
                    .Include(item => item.Category)
                    .ThenInclude(item => item.Image)
                    .Where(item => item.Date == jumpStart.DateTime.Date)
                    .OrderBy(item => item.Order).ToListAsync();

            var pdfData = await _puppeteerService.BuildPdfAsync(jumpStart.DateTime, articles);

            await UploadPdfAsync(jumpStart, pdfData);

            jumpStart.HasPdf = true;
            _dbContext.Update(jumpStart);

            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<EmailRecipientService>(service =>
                service.CreateForJumpStart(jumpStart.Id));
        }

        public Task<string> CreatePdfUrlAsync(JumpStart jumpStart)
        {
            return _googleStorageService.CreateSignedUrlAsync(GetName(jumpStart), 24);
        }

        private async Task UploadPdfAsync(JumpStart jumpStart, byte[] pdfData)
        {
            await _googleStorageService.UploadPdfAsync(GetName(jumpStart), pdfData);
        }

        private string GetName(JumpStart jumpStart)
        {
            return $"volkswagen_pdf/{jumpStart.DateTime.Date:yyyy_MM_dd}.pdf";
        }
    }
}