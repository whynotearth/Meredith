using System.Collections.Generic;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Services;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class ClientListResult
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

        public ClientListResult(Client client, FormSignature? formSignature, IFileService fileService)
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
}