using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace WhyNotEarth.Meredith.GoogleCloud
{
    public class GoogleStorageService
    {
        private readonly GoogleCloudOptions _options;

        public GoogleStorageService(IOptions<GoogleCloudOptions> options)
        {
            _options = options.Value;
        }

        public async Task UploadPdfAsync(string objectName, Stream content)
        {
            using var storageClient = await StorageClient.CreateAsync(GetCredential());

            await storageClient.UploadObjectAsync(_options.StorageBucket, objectName, "application/pdf", content);
        }

        public Task<string> CreateSignedUrlAsync(string objectName, int hours)
        {
            var urlSigner = UrlSigner.FromServiceAccountCredential(GetServiceAccount());

            return urlSigner.SignAsync(_options.StorageBucket, objectName, TimeSpan.FromHours(hours), HttpMethod.Get);
        }

        private GoogleCredential GetCredential()
        {
            return GoogleCredential.FromServiceAccountCredential(GetServiceAccount());
        }

        private ServiceAccountCredential GetServiceAccount()
        {
            var initializer = new ServiceAccountCredential.Initializer(_options.ClientEmail)
            {
                ProjectId = _options.ProjectId
            };

            return new ServiceAccountCredential(initializer.FromPrivateKey(_options.PrivateKey));
        }
    }
}