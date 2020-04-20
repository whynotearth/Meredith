using System;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen.Recipient
{
    public class RecipientResult
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public DateTime CreationDateTime { get; set; }

        public RecipientResult(Data.Entity.Models.Modules.Volkswagen.Recipient recipient)
        {
            Id = recipient.Id;
            Email = recipient.Email;
            CreationDateTime = recipient.CreationDateTime;
        }
    }
}