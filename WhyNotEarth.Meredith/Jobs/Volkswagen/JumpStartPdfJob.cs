using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.GoogleCloud;
using WhyNotEarth.Meredith.Jobs.Public;
using WhyNotEarth.Meredith.Pdf;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.Jobs.Volkswagen
{
    public class JumpStartPdfJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IDbContext _dbContext;
        private readonly GoogleStorageService _googleStorageService;
        private readonly IHtmlService _htmlService;
        private readonly JumpStartEmailTemplateService _jumpStartEmailTemplateService;

        public JumpStartPdfJob(IDbContext dbContext, GoogleStorageService googleStorageService,
            IBackgroundJobClient backgroundJobClient, JumpStartEmailTemplateService jumpStartEmailTemplateService,
            IHtmlService htmlService)
        {
            _dbContext = dbContext;
            _googleStorageService = googleStorageService;
            _backgroundJobClient = backgroundJobClient;
            _jumpStartEmailTemplateService = jumpStartEmailTemplateService;
            _htmlService = htmlService;
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

            var emailTemplate = _jumpStartEmailTemplateService.GetPdfHtml(jumpStart.DateTime, articles);
            var pdfData = await _htmlService.ToPdfAsync(emailTemplate);

            await UploadPdfAsync(jumpStart, pdfData);

            jumpStart.HasPdf = true;
            _dbContext.JumpStarts.Update(jumpStart);

            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<EmailRecipientJob>(job =>
                job.CreateForJumpStart(jumpStart.Id));
        }

        public Task<string> CreatePdfUrlAsync(JumpStart jumpStart)
        {
            return _googleStorageService.CreateSignedUrlAsync(GetName(jumpStart), 24);
        }

        private async Task UploadPdfAsync(JumpStart jumpStart, byte[] pdfData)
        {
            await _googleStorageService.UploadFileAsync(GetName(jumpStart), "application/pdf",
                new MemoryStream(pdfData));
        }

        private string GetName(JumpStart jumpStart)
        {
            return $"volkswagen_pdf/{jumpStart.DateTime.Date:yyyy_MM_dd}.pdf";
        }
    }
}