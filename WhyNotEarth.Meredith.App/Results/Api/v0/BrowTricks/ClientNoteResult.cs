using System;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class ClientNoteResult
    {
        public int Id { get; }

        public string Note { get; }

        public DateTime CreatedAt { get; }

        public ClientNoteResult(ClientNote clientNote, int maxLength)
        {
            Id = clientNote.Id;
            Note = Truncate(clientNote.Note, maxLength);
            CreatedAt = clientNote.CreatedAt;
        }

        public ClientNoteResult(ClientNote clientNote)
        {
            Id = clientNote.Id;
            Note = clientNote.Note;
            CreatedAt = clientNote.CreatedAt;
        }


        private string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}