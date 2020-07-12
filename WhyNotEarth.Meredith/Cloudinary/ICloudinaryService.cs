using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.Cloudinary
{
    public interface ICloudinaryService
    {
        Task DeleteAsync(string imageUrl);
    }
}