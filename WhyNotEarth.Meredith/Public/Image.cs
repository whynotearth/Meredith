using System;

namespace WhyNotEarth.Meredith.Public
{
    public class Image
    {
        public int Id { get; set; }

        public string? CloudinaryPublicId { get; set; }

        public string Url { get; set; } = null!;

        public string? Title { get; set; }

        public string? AltText { get; set; }

        public int Order { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        /// <summary>
        ///     File size in bytes
        /// </summary>
        public long? FileSize { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}