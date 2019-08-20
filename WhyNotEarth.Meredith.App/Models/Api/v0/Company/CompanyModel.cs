namespace WhyNotEarth.Meredith.App.Models.Api.v0.Company
{
    using System.ComponentModel.DataAnnotations;

    public class CompanyModel
    {
        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
