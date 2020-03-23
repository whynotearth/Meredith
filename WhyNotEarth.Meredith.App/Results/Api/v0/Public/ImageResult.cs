namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public
{
    public class ImageResult
    {
        public int Order { get; }

        public string Url { get; }

        public ImageResult(int order, string url)
        {
            Order = order;
            Url = url;
        }
    }
}