using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class PostPreviewModel
    {
        [Required]
        public List<int> PostIds { get; set; } = null!;
    }
}