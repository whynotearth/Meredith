namespace WhyNotEarth.Meredith.Public
{
    public class Video
    {
        public int Id { get; set; }

        public string CloudinaryPublicId { get; set; } = null!;

        public string Url { get; set; } = null!;

        public int Width { get; set; }

        public int Height { get; set; }
        
        public double? FileSize { get; set; } 

        public double Duration { get; set; }

        public string Format { get; set; } = null!;

        public string ThumbnailUrl { get; set; } = null!;
    }
}