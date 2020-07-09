using System.Collections.Generic;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class ClientResult
    {
        public int Id { get; }

        public string Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public List<NotificationType> NotificationTypes { get; set; }

        public string? Notes { get; set; }

        public string? AvatarUrl { get; set; }

        public ClientResult(Client client)
        {
            Id = client.Id;
            Email = client.User.Email;
            FirstName = client.User.FirstName;
            LastName = client.User.LastName;
            PhoneNumber = client.User.PhoneNumber;
            NotificationTypes = client.NotificationType.ToList();
            AvatarUrl = client.User.ImageUrl;
        }
    }
}