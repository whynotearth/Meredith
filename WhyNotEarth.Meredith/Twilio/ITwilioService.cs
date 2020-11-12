using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.Twilio
{
    public interface ITwilioService
    {
        Task SendAsync(int shortMessageId);

        Task SendAsync(ShortMessage message);
    }
}