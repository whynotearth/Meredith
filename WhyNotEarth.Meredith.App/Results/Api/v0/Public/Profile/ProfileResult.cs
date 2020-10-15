using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Profile
{
    public class ProfileResult
    {
        public string UserName { get; }

        public string Email { get; }

        public string? FirstName { get; }

        public string? LastName { get; }

        public string? PhoneNumber { get; }

        public string? Address { get; }

        public string? GoogleLocation { get; }

        public string? ImageUrl { get; }

        public bool IsEmailConfirmed { get; }

        public bool IsPhoneNumberConfirmed { get; }

        public ProfileResult(User user)
        {
            UserName = user.UserName;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            PhoneNumber = user.PhoneNumber;
            Address = user.Address;
            GoogleLocation = user.GoogleLocation;
            ImageUrl = user.ImageUrl;
            IsPhoneNumberConfirmed = user.PhoneNumberConfirmed;
            IsEmailConfirmed = user.EmailConfirmed;
        }
    }
}