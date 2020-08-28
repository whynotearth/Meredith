using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class ClientGetResult
    {
        public int Id { get; }

        public string Email { get; }

        public string? FirstName { get; }

        public string? LastName { get; }

        public string? PhoneNumber { get; }

        public List<NotificationType> NotificationTypes { get; }

        public string? AvatarUrl { get; }

        public PmuStatusType PmuStatus { get; }

        public string? PmuPdfUrl { get; }

        public List<ImageResult>? Images { get; }

        public List<VideoResult>? Videos { get; }

        public ClientGetResult(Client client, string? pmuPdfUrl, List<ClientImage> images, List<ClientVideo> videos)
        {
            Id = client.Id;
            Email = client.User.Email;
            FirstName = client.User.FirstName;
            LastName = client.User.LastName;
            PhoneNumber = client.User.PhoneNumber;
            NotificationTypes = client.NotificationType.ToList();
            AvatarUrl = client.User.ImageUrl;
            PmuStatus = client.PmuStatus;
            PmuPdfUrl = pmuPdfUrl;
            Images = images.Select(item => new ImageResult(item)).ToList();
            Videos = videos.Select(item => new VideoResult(item)).ToList();
        }
    }
}