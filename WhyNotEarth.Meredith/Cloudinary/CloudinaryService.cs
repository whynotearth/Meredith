using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.Cloudinary.Models;
using WhyNotEarth.Meredith.Public;
using Video = WhyNotEarth.Meredith.Public.Video;

namespace WhyNotEarth.Meredith.Cloudinary
{
    internal class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryOptions _options;

        public CloudinaryService(IOptions<CloudinaryOptions> options)
        {
            _options = options.Value;
        }

        public Task DeleteByUrlAsync(string imageUrl)
        {
            // TODO: This is not safe, we should not do this
            var publicId = Path.GetFileNameWithoutExtension(imageUrl);

            return DeleteAsync(publicId);
        }

        public Task DeleteAsync(string publicId)
        {
            var cloudinary = new CloudinaryDotNet.Cloudinary(new Account(_options.CloudName,
                _options.ApiKey, _options.ApiSecret));

            var deleteParams = new DeletionParams(publicId);
            return cloudinary.DestroyAsync(deleteParams);
        }

        public async Task<List<T>> GetUpdatedValueAsync<T>(List<T>? oldValues, List<CloudinaryImageModel>? models)
            where T : Image, new()
        {
            // Delete the removed ones from Cloudinary
            if (oldValues != null)
            {
                foreach (var oldValue in oldValues)
                {
                    if (models?.Any(item => item.PublicId == oldValue.CloudinaryPublicId) != true)
                    {
                        await DeleteAsync(oldValue.CloudinaryPublicId!);
                    }
                }
            }

            var result = new List<T>();

            if (models is null)
            {
                return result;
            }

            foreach (var model in models)
            {
                var oldValue = oldValues?.FirstOrDefault(image => image.CloudinaryPublicId == model.PublicId);

                if (oldValue != null)
                {
                    result.Add(oldValue);
                }
                else
                {
                    // First fetch image details from cloudinary to retrieve filesize.
                    var resource = GetCloudinaryResourcePerPublicId(model.PublicId, ResourceType.Image);

                    result.Add(new T
                    {
                        CloudinaryPublicId = model.PublicId,
                        Url = model.Url,
                        Width = model.Width,
                        Height = model.Height,
                        FileSize = resource?.Bytes,
                    });
                }
            }

            return result;
        }

        public async Task<List<T>> GetUpdatedValueAsync<T>(List<T>? oldValues, List<CloudinaryVideoModel>? models)
            where T : Video, new()
        {
            // Delete the removed ones from Cloudinary
            if (oldValues != null)
            {
                foreach (var oldValue in oldValues)
                {
                    if (models?.Any(item => item.PublicId == oldValue.CloudinaryPublicId) != true)
                    {
                        await DeleteAsync(oldValue.CloudinaryPublicId!);
                    }
                }
            }

            var result = new List<T>();

            if (models is null)
            {
                return result;
            }

            foreach (var model in models)
            {
                var oldValue = oldValues?.FirstOrDefault(image => image.CloudinaryPublicId == model.PublicId);

                if (oldValue != null)
                {
                    result.Add(oldValue);
                }
                else
                {
                    // First fetch video details from cloudinary to retrieve filesize.
                    var resource = GetCloudinaryResourcePerPublicId(model.PublicId, ResourceType.Video);

                    result.Add(new T
                    {
                        CloudinaryPublicId = model.PublicId,
                        Url = model.Url,
                        Width = model.Width!.Value,
                        Height = model.Height!.Value,
                        FileSize = resource?.Bytes,
                        Duration = model.Duration!.Value,
                        Format = model.Format,
                        ThumbnailUrl = model.ThumbnailUrl
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Queries the cloudinary api to get the resource per it's public id.
        /// </summary>
        /// <param name="publicId"></param>
        /// <param name="resourceType">The resource type of the resource to fetch.</param>
        /// <returns></returns>
        private GetResourceResult GetCloudinaryResourcePerPublicId(string publicId, ResourceType resourceType)
        {
            var cloudinary = new CloudinaryDotNet.Cloudinary(new Account(_options.CloudName,
                _options.ApiKey, _options.ApiSecret));

            var par = new GetResourceParams(publicId);
            par.ResourceType = resourceType;
            
            var result = cloudinary.GetResource(par);

            return result;
        }
    }
}