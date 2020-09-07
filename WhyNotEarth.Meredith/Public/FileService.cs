using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.GoogleCloud;
using WhyNotEarth.Meredith.Services;

namespace WhyNotEarth.Meredith.Public
{
    internal class FileService : IFileService
    {
        private const string DirectorySeparator = "/";

        private readonly GoogleStorageService _googleStorageService;

        public FileService(GoogleStorageService googleStorageService)
        {
            _googleStorageService = googleStorageService;
        }

        public Task SaveAsync(string path, string contentType, Stream stream)
        {
            return _googleStorageService.UploadFileAsync(path, contentType, stream);
        }

        public async Task<string> SaveAsync(string companySlug, List<string> path, string contentType, Stream stream)
        {
            var finalPath = GetPath(companySlug, path);

            await _googleStorageService.UploadFileAsync(finalPath, contentType, stream);

            return finalPath;
        }

        public async Task GetAsync(string path, Stream stream)
        {
            await _googleStorageService.DownloadFileAsync(path, stream);
        }

        public string GetPrivateUrl(string path)
        {
            return _googleStorageService.CreateSignedUrl(path, 24);
        }

        private string GetPath(string companySlug, IEnumerable<string> path)
        {
            var directories = path.ToList();
            directories.Insert(0, companySlug);

            return string.Join(DirectorySeparator, directories);
        }
    }
}