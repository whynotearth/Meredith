using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Tenant.Models
{
    public class TenantImageModel
    {
        public TenantImageModel(TenantImage image)
        {
            Height = image.Height;
            Width = image.Width;
            Order = image.Order;
            Title = image.Title;
            Url = image.Url;
            AltText = image.AltText;
            FileSize = image.FileSize;
            CloudinaryPublicId = image.CloudinaryPublicId;
        }
        
        public int Id { get; set; }

        public string? CloudinaryPublicId { get; set; }

        public string Url { get; set; } = null!;

        public string? Title { get; set; }

        public string? AltText { get; set; }

        public int Order { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }
        
        public long? FileSize { get; set; }
    }
}