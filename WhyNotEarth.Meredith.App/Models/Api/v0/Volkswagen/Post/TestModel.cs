using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen.Post
{
    public class TestModel
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
