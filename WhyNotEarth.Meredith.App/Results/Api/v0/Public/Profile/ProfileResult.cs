using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Profile
{
    public class ProfileResult
    {
        public string Email { get; }

        public string Name { get; }

        public ProfileResult(User user)
        {
            Email = user.Email;

            Name = user.Name;
        }
    }
}