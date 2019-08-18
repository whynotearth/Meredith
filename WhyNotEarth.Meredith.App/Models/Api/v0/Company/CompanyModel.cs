using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Company
{
    public class CompanyModel
    {
        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
