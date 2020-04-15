using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public
{
    public class ImageResult
    {
        public int Order { get; }

        public string Url { get; }

        public ImageResult(Image image)
        {
            Order = image.Order;
            Url = image.Url;
        }
    }
}