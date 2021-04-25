using System.Collections.Generic;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Authentication
{
    public class PingResult
    {
        public int Id { get; }

        public string UserName { get; }

        // Breaking change, remove
        public bool IsAuthenticated { get; }

        public bool IsPhoneNumberConfirmed { get; }

        public bool IsEmailConfirmed { get; }

        public List<string> LoginProviders { get; }

        public bool IsAdmin { get; }

        public PingResult(User user, List<string> loginProviders, IList<string>? roles)
        {
            Id = user.Id;
            UserName = user.UserName;
            IsAuthenticated = true;
            IsPhoneNumberConfirmed = user.PhoneNumberConfirmed;
            IsEmailConfirmed = user.EmailConfirmed;
            LoginProviders = loginProviders;
            IsAdmin = roles?.Contains("SuperAdmin") ?? false;
        }
    }
}