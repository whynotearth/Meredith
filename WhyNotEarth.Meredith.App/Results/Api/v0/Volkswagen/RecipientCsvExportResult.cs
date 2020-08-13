using System;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class RecipientResult
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime CreationDateTime { get; set; }

        public RecipientResult(Recipient recipient)
        {
            Id = recipient.Id;
            Email = recipient.Email;
            CreationDateTime = recipient.CreatedAt;
            FirstName = recipient.FirstName;
            LastName = recipient.LastName;
        }
    }
}