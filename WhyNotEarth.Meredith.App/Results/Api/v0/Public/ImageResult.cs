using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public
{
    public class ImageResult
    {
        public string? PublicId { get; }

        public string Url { get; }

        public int Order { get; }

        public int? Width { get; }

        public int? Height { get; }

        public ImageResult(Image image)
        {
            PublicId = image.CloudinaryPublicId;
            Url = image.Url;
            Order = image.Order;
            Width = image.Width;
            Height = image.Height;
        }
    }
}