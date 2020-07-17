namespace WhyNotEarth.Meredith.GoogleCloud
{
    public class GoogleCloudOptions
    {
        public string ProjectId { get; set; } = null!;

        public string ClientEmail { get; set; } = null!;

        public string PrivateKey { get; set; } = null!;

        public string StorageBucket { get; set; } = null!;
    }
}