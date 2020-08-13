using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Cloudinary.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Cloudinary
{
    public interface ICloudinaryService
    {
        Task DeleteAsync(string publicId);

        Task DeleteByUrlAsync(string imageUrl);

        Task<Image?> GetUpdatedValueAsync(Image? oldValue, CloudinaryImageModel? model);

        Task<List<T>> GetUpdatedValueAsync<T>(List<T>? oldValues, List<CloudinaryImageModel>? models) where T : Image, new();

        Task<List<T>> GetUpdatedValueAsync<T>(List<T>? oldValues, List<CloudinaryVideoModel>? models) where T : Video, new();
    }
}