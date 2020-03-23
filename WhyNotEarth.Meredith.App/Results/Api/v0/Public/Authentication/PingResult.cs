using System.Collections.Generic;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Authentication
{
    public class PingResult
    {
        public int Id { get; }

        public string UserName { get; }

        public bool IsAuthenticated { get; }

        public List<string> LoginProviders { get; }

        public PingResult(int id, string userName, bool isAuthenticated, List<string> loginProviders)
        {
            Id = id;
            UserName = userName;
            IsAuthenticated = isAuthenticated;
            LoginProviders = loginProviders;
        }
    }
}