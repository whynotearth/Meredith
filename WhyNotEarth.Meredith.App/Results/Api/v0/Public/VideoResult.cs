using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public
{
    public class VideoResult
    {
        public string? PublicId { get; }

        public string Url { get; }

        public VideoResult(Video video)
        {
            PublicId = video.CloudinaryPublicId;
            Url = video.Url;
        }
    }
}