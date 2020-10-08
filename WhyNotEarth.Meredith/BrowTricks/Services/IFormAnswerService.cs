using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    public interface IFormAnswerService
    {
        Task<byte[]> GetPngAsync(int formTemplateId, User user);

        Task<byte[]> GetPdfAsync(int formTemplateId, User user);

        Task<byte[]> GetPngAsync(int formTemplateId, FormSignatureModel model, User user);

        Task<byte[]> GetPngAsync(int formTemplateId, int clientId, FormSignatureModel model, User user);

        Task SubmitAsync(int formTemplateId, int clientId, FormSignatureModel model, User user);

        Task SendNotificationAsync(int formTemplateId, int clientId, User user, string callbackUrl);
    }
}