using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Cloudinary;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    internal class TenantMediaService : ITenantMediaService
    {
        private readonly IClientService _clientService;
        private readonly TenantService _tenantService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IDbContext _dbContext;

        public TenantMediaService(IDbContext dbContext, ICloudinaryService cloudinaryService,
            IClientService clientService, TenantService tenantService)
        {
            _dbContext = dbContext;
            _cloudinaryService = cloudinaryService;
            _clientService = clientService;
            _tenantService = tenantService;
        }

        public async Task CreateImageAsync(string tenantSlug, BrowTricksImageModel model, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            if (model.ClientId.HasValue)
            {
                var client = await _clientService.ValidateOwnerAsync(model.ClientId.Value, user);

                if (client.TenantId != tenant.Id)
                {
                    throw new ForbiddenException();
                }
            }

            _dbContext.Images.Add(new BrowTricksImage
            {
                CloudinaryPublicId = model.Image.PublicId,
                TenantId = tenant.Id,
                ClientId = model.ClientId,
                Url = model.Image.Url,
                Width = model.Image.Width,
                Height = model.Image.Height,
                FileSize = model.Image.FileSize,
                Description = model.Description,
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(int imageId, User user)
        {
            var image = await _dbContext.Images.OfType<BrowTricksImage>()
                .Include(item => item.Tenant)
                .FirstOrDefaultAsync(item => item.Id == imageId);

            if (image is null)
            {
                throw new RecordNotFoundException($"Image {imageId} not found");
            }

            if (image.Tenant!.OwnerId != user.Id)
            {
                throw new ForbiddenException("You don't own this tenant");
            }

            await _cloudinaryService.DeleteAsync(image.CloudinaryPublicId!);

            _dbContext.Images.Remove(image);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateVideoAsync(string tenantSlug, BrowTricksVideoModel model, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            if (model.ClientId.HasValue)
            {
                var client = await _clientService.ValidateOwnerAsync(model.ClientId.Value, user);

                if (client.TenantId != tenant.Id)
                {
                    throw new ForbiddenException();
                }
            }

            _dbContext.Videos.Add(new BrowTricksVideo
            {
                CloudinaryPublicId = model.Video.PublicId,
                TenantId = tenant.Id,
                ClientId = model.ClientId,
                Url = model.Video.Url,
                Width = model.Video.Width.Value,
                Height = model.Video.Height.Value,
                FileSize = model.Video.FileSize,
                Duration = model.Video.Duration.Value,
                Format = model.Video.Format,
                ThumbnailUrl = model.Video.ThumbnailUrl,
                Description = model.Description,
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteVideoAsync(int videoId, User user)
        {
            var video = await _dbContext.Videos.OfType<BrowTricksVideo>()
                .Include(item => item.Tenant)
                .FirstOrDefaultAsync(item => item.Id == videoId);

            if (video is null)
            {
                throw new RecordNotFoundException($"Video {videoId} not found");
            }

            if (video.Tenant!.OwnerId != user.Id)
            {
                throw new ForbiddenException("You don't own this tenant");
            }

            await _cloudinaryService.DeleteAsync(video.CloudinaryPublicId!);

            _dbContext.Videos.Remove(video);
            await _dbContext.SaveChangesAsync();
        }
    }
}