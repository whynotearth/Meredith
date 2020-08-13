using System.Threading.Tasks;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Twilio
{
    public interface ITwilioService
    {
        Task SendAsync(int shortMessageId);

        Task SendAsync(ShortMessage message);
    }
}