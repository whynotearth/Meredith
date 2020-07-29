using System.Collections.Generic;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class ClientResult
    {
        public int Id { get; }

        public string Email { get; }

        public string? FirstName { get; }

        public string? LastName { get; }

        public string? PhoneNumber { get; }

        public List<NotificationType> NotificationTypes { get; }

        public string? AvatarUrl { get; }

        public bool IsPmuCompleted { get; }

        public string? PmuPdfUrl { get; }

        public ClientResult(Client client, string? pmuPdfUrl)
        {
            Id = client.Id;
            Email = client.User.Email;
            FirstName = client.User.FirstName;
            LastName = client.User.LastName;
            PhoneNumber = client.User.PhoneNumber;
            NotificationTypes = client.NotificationType.ToList();
            AvatarUrl = client.User.ImageUrl;
            IsPmuCompleted = client.IsPmuCompleted;
            PmuPdfUrl = pmuPdfUrl;
        }
    }
}