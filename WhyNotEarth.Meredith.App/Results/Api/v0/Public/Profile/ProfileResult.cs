using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Profile
{
    public class ProfileResult
    {
        public string Email { get; }

        public string? FirstName { get; }

        public string? LastName { get; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? GoogleLocation { get; set; }

        public string? ImageUrl { get; set; }

        public ProfileResult(User user)
        {
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            PhoneNumber = user.PhoneNumber;
            Address = user.Address;
            GoogleLocation = user.GoogleLocation;
            ImageUrl = user.ImageUrl;
        }
    }
}