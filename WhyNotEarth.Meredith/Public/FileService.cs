using System.IO;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.GoogleCloud;
using WhyNotEarth.Meredith.Services;

namespace WhyNotEarth.Meredith.Public
{
    internal class FileService : IFileService
    {
        private readonly GoogleStorageService _googleStorageService;

        public FileService(GoogleStorageService googleStorageService)
        {
            _googleStorageService = googleStorageService;
        }

        public Task SaveAsync(string path, string contentType, Stream stream)
        {
            return _googleStorageService.UploadFileAsync(path, contentType, stream);
        }

        public async Task GetAsync(string path, Stream stream)
        {
            await _googleStorageService.DownloadFileAsync(path, stream);
        }

        public Task<string> GetPrivateUrlAsync(string path)
        {
            return _googleStorageService.CreateSignedUrlAsync(path, 24);
        }
    }
}