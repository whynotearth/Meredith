namespace WhyNotEarth.Meredith.App.Models.Api.v0.Authentication
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterModel
    {
        [Required]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
