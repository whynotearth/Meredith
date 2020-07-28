using System;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class RecipientCsvExportResult
    {
        public string Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime CreationDateTime { get; set; }

        public RecipientCsvExportResult(Data.Entity.Models.Modules.Volkswagen.Recipient recipient)
        {
            Email = recipient.Email;
            FirstName = recipient.FirstName;
            LastName = recipient.LastName;
            CreationDateTime = recipient.CreatedAt;
        }
    }
}