using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Services;

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

        public ClientGetResult(Client client, FormSignature? formSignature, IFileService fileService)
        {
            Id = client.Id;
            Email = client.User.Email;
            FirstName = client.User.FirstName;
            LastName = client.User.LastName;
            PhoneNumber = client.User.PhoneNumber;
            NotificationTypes = client.NotificationType.ToList();
            AvatarUrl = client.User.ImageUrl;
            PmuStatus = GetStatus(formSignature);
            PmuPdfUrl = PmuStatus == PmuStatusType.Completed
                ? fileService.GetPrivateUrl(formSignature!.PdfPath!)
                : null;
            Images = client.Images?.Select(item => new ImageResult(item)).ToList();
            Videos = client.Videos?.Select(item => new VideoResult(item)).ToList();
        }

        private PmuStatusType GetStatus(FormSignature? formSignature)
        {
            if (formSignature is null)
            {
                return PmuStatusType.Incomplete;
            }

            if (formSignature.PdfPath != null)
            {
                return PmuStatusType.Completed;
            }

            return PmuStatusType.Saving;
        }
    }

    public enum PmuStatusType
    {
        [EnumMember(Value = "incomplete")] Incomplete = 1,

        [EnumMember(Value = "saving")] Saving = 2,

        [EnumMember(Value = "completed")] Completed = 3
    }
}