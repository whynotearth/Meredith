using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Cloudinary.Models;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.Cloudinary
{
    public interface ICloudinaryService
    {
        Task DeleteAsync(string publicId);

        Task DeleteByUrlAsync(string imageUrl);

        Task<Image?> GetUpdatedValueAsync(Image? oldValue, CloudinaryImageModel? model);

        Task<List<Image>> GetUpdatedValueAsync(List<Image>? oldValues, List<CloudinaryImageModel>? models);

        Task<List<Video>> GetUpdatedValueAsync(List<Video>? oldValues, List<CloudinaryVideoModel>? models);
    }
}