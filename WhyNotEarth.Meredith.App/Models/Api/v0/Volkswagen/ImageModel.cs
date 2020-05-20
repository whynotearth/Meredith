using WhyNotEarth.Meredith.App.Validation;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class ImageModel
    {
        [Mandatory]
        public string Url { get; set; } = null!;

        public int? Width { get; set; }

        public int? Height { get; set; }
    }
}