using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Tenant
{
    public class UserListResult
    {
        public char Group { get; }

        public List<UserResult> Clients { get; }

        public UserListResult(char group, IEnumerable<User> users)
        {
            Group = group;
            Clients = users.Select(item => new UserResult(item)).ToList();
        }
    }

    public class UserResult
    {
        public string Name { get; }

        public string PhoneNumber { get; }

        public string Email { get; }

        public UserResult(User user)
        {
            Name = user.Name;
            PhoneNumber = user.PhoneNumber;
            Email = user.Email;
        }
    }
}