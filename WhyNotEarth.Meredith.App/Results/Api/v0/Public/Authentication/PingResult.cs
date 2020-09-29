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

        public List<string> LoginProviders { get; }

        public PingResult(User user, List<string> loginProviders)
        {
            Id = user.Id;
            UserName = user.UserName;
            IsAuthenticated = true;
            IsPhoneNumberConfirmed = user.PhoneNumberConfirmed;
            LoginProviders = loginProviders;
        }
    }
}