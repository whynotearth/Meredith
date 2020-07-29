using System.Threading.Tasks;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.Twilio
{
    public interface ITwilioService
    {
        Task SendAsync(int shortMessageId);

        Task SendAsync(ShortMessage message);
    }
}