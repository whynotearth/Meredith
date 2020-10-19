using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.Cloudinary;

namespace WhyNotEarth.Meredith.BrowTricks.Jobs
{
    public class FileSizeMigrationJob
    {
        private readonly IDbContext _dbContext;
        private readonly CloudinaryOptions _options;

        public FileSizeMigrationJob(IDbContext dbContext, IOptions<CloudinaryOptions> options)
        {
            _dbContext = dbContext;
            _options = options.Value;
        }

        public async Task RunAsync()
        {
            var formTemplates = await _dbContext.FormTemplates.Where(item => item.Name == "Pre and Post Care Agreement").ToListAsync();

            _dbContext.FormTemplates.RemoveRange(formTemplates);
            await _dbContext.SaveChangesAsync();

            var images = await _dbContext.Images.OfType<BrowTricksImage>()
                .Where(item => item.CloudinaryPublicId != null && item.FileSize == null).ToListAsync();

            foreach (var clientImage in images)
            {
                var size = await GetSizeAsync(clientImage.CloudinaryPublicId!, ResourceType.Image);
                clientImage.FileSize = size;

                _dbContext.Images.Update(clientImage);
            }

            await _dbContext.SaveChangesAsync();

            var videos = await _dbContext.Videos.OfType<BrowTricksVideo>()
                .Where(item => item.FileSize == null).ToListAsync();

            foreach (var clientVideo in videos)
            {
                var size = await GetSizeAsync(clientVideo.CloudinaryPublicId!, ResourceType.Video);
                clientVideo.FileSize = size;

                _dbContext.Videos.Update(clientVideo);
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<long> GetSizeAsync(string publicId, ResourceType resourceType)
        {
            var cloudinary = new CloudinaryDotNet.Cloudinary(new Account(_options.CloudName,
                _options.ApiKey, _options.ApiSecret));

            var par = new GetResourceParams(publicId)
            {
                ResourceType = resourceType
            };

            var resource = await cloudinary.GetResourceAsync(par);

            return resource.Bytes;
        }
    }
}