using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public
{
    public class ImageResult
    {
        public int Id { get; }

        public int Order { get; }

        public string Url { get; }

        public ImageResult(Image image)
        {
            Id = image.Id;
            Order = image.Order;
            Url = image.Url;
        }
    }
}