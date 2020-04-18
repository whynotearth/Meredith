using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class MemoModel
    {
        [Required]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public string DistributionGroup { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public string Description { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    }
}
