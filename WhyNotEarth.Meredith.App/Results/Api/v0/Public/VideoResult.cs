using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public
{
    public class VideoResult
    {
        public string? PublicId { get; }

        public string Url { get; }

        public int Width { get; }

        public int Height { get; }

        public double Duration { get; }

        public string Format { get; }

        public string ThumbnailUrl { get; }

        public VideoResult(Video video)
        {
            PublicId = video.CloudinaryPublicId;
            Url = video.Url;
            Width = video.Width;
            Height = video.Height;
            Duration = video.Duration;
            Format = video.Format;
            ThumbnailUrl = video.ThumbnailUrl;
        }
    }
}