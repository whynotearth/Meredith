
using System.Collections.Generic;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class ClientListResult
    {
        public List<ClientResult> Records { get; set; } = new List<ClientResult>();

        public int Total {get;set;}

        public int CurrentPage {get;set;}

        public int PerPage {get;set;}
        
        public int Pages {get;set;}

        public class ClientResult
        {
            public int Id { get; }

            public string Email { get; }

            public string? FirstName { get; }

            public string? LastName { get; }

            public string? PhoneNumber { get; }

            public string? AvatarUrl { get; }

            public ClientResult(Client client)
            {
                Id = client.Id;
                Email = client.Email;
                FirstName = client.FirstName;
                LastName = client.LastName;
                PhoneNumber = client.PhoneNumber;
                AvatarUrl = client.AvatarUrl;
            }

        }
    }
}