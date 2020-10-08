using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Cloudinary;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    internal class ClientMediaService : IClientMediaService
    {
        private readonly IClientService _clientService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IDbContext _dbContext;

        public ClientMediaService(IDbContext dbContext, ICloudinaryService cloudinaryService,
            IClientService clientService)
        {
            _dbContext = dbContext;
            _cloudinaryService = cloudinaryService;
            _clientService = clientService;
        }

        public async Task CreateImageAsync(ClientImageModel model, User user)
        {
            var client = await _clientService.ValidateOwnerAsync(model.ClientId.Value, user);

            client.Images ??= new List<ClientImage>();

            client.Images.Add(new ClientImage
            {
                CloudinaryPublicId = model.Image.PublicId,
                Url = model.Image.Url,
                Width = model.Image.Width,
                Height = model.Image.Height,
                FileSize = model.Image.FileSize,
                Description = model.Description
            });

            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(int imageId, User user)
        {
            var clientImage = await _dbContext.Images.OfType<ClientImage>()
                .Include(item => item.Client)
                .ThenInclude(item => item!.Tenant)
                .FirstOrDefaultAsync(item => item.Id == imageId);

            if (clientImage is null)
            {
                throw new RecordNotFoundException($"Image {imageId} not found");
            }

            if (clientImage.Client?.Tenant.OwnerId != user.Id)
            {
                throw new ForbiddenException("You don't own this tenant");
            }

            await _cloudinaryService.DeleteAsync(clientImage.CloudinaryPublicId!);

            _dbContext.Images.Remove(clientImage);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateVideoAsync(ClientVideoModel model, User user)
        {
            var client = await _clientService.ValidateOwnerAsync(model.ClientId.Value, user);

            client.Videos ??= new List<ClientVideo>();

            client.Videos.Add(new ClientVideo
            {
                CloudinaryPublicId = model.Video.PublicId,
                Url = model.Video.Url,
                Width = model.Video.Width.Value,
                Height = model.Video.Height.Value,
                FileSize = model.Video.FileSize,
                Duration = model.Video.Duration.Value,
                Format = model.Video.Format,
                ThumbnailUrl = model.Video.ThumbnailUrl,
                Description = model.Description
            });

            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteVideoAsync(int videoId, User user)
        {
            var clientVideo = await _dbContext.Videos.OfType<ClientVideo>()
                .Include(item => item.Client)
                .ThenInclude(item => item!.Tenant)
                .FirstOrDefaultAsync(item => item.Id == videoId);

            if (clientVideo is null)
            {
                throw new RecordNotFoundException($"Video {videoId} not found");
            }

            if (clientVideo.Client?.Tenant.OwnerId != user.Id)
            {
                throw new ForbiddenException("You don't own this tenant");
            }

            await _cloudinaryService.DeleteAsync(clientVideo.CloudinaryPublicId!);

            _dbContext.Videos.Remove(clientVideo);
            await _dbContext.SaveChangesAsync();
        }
    }
}