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

        public List<FormSignatureResult> Signatures { get; }

        public List<ImageResult>? Images { get; }

        public List<VideoResult>? Videos { get; }

        public ClientGetResult(Client client, Dictionary<int, string?> signatureUrls)
        {
            Id = client.Id;
            Email = client.User.Email;
            FirstName = client.User.FirstName;
            LastName = client.User.LastName;
            PhoneNumber = client.User.PhoneNumber;
            NotificationTypes = client.NotificationType.ToList();
            AvatarUrl = client.User.ImageUrl;
            Signatures = signatureUrls.Select(item => new FormSignatureResult(item.Key, item.Value)).ToList();
            Images = client.Images?.Select(item => new ImageResult(item)).ToList();
            Videos = client.Videos?.Select(item => new VideoResult(item)).ToList();
        }

        public class FormSignatureResult
        {
            public int FormTemplateId { get; }

            public string? PdfUrl { get; }

            public FormSignatureResult(int formTemplateId, string? pdfUrl)
            {
                FormTemplateId = formTemplateId;
                PdfUrl = pdfUrl;
            }
        }
    }
}