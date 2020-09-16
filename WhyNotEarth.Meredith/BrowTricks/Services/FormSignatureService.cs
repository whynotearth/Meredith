using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Services;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    internal class FormSignatureService : IFormSignatureService
    {
        private readonly IDbContext _dbContext;
        private readonly IFileService _fileService;

        public FormSignatureService(IDbContext dbContext, IFileService fileService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        public async Task<List<string>> GetSignatureUrlsAsync(Client client)
        {
            var formSignatures = await _dbContext.FormSignatures
                .Where(item => item.ClientId == client.Id && item.PdfPath != null).ToListAsync();

            return formSignatures.Select(item => _fileService.GetPrivateUrl(item.PdfPath!)).ToList();
        }
    }
}