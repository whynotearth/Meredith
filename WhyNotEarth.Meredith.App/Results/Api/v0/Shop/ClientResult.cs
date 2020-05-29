using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Shop
{
    public class ClientListResult
    {
        public char Group { get; }

        public List<ClientResult> Clients { get; }

        public ClientListResult(char group, IEnumerable<Client> clients)
        {
            Group = group;
            Clients = clients.Select(item => new ClientResult(item)).ToList();
        }
    }

    public class ClientResult
    {
        public string Name { get; }

        public string PhoneNumber { get; }

        public string Email { get; }

        public ClientResult(Client client)
        {
            Name = client.User.Name;
            PhoneNumber = client.User.PhoneNumber;
            Email = client.User.Email;
        }
    }
}