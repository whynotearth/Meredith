using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace WhyNotEarth.Meredith.Cloudinary
{
    internal class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryOptions _options;

        public CloudinaryService(IOptions<CloudinaryOptions> options)
        {
            _options = options.Value;
        }

        public Task DeleteAsync(string imageUrl)
        {
            var cloudinary = new CloudinaryDotNet.Cloudinary(new Account(_options.CloudName,
                _options.ApiKey, _options.ApiSecret));

            // TODO: This is not safe, we should not do this
            var publicId = Path.GetFileNameWithoutExtension(imageUrl);

            var deleteParams = new DeletionParams(publicId);
            return cloudinary.DestroyAsync(deleteParams);
        }
    }
}