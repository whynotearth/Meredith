namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Video
    {
        public int Id { get; set; }

        public string CloudinaryPublicId { get; set; } = null!;

        public string Url { get; set; } = null!;
    }
}