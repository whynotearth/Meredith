using WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class ClientNoteResult
    {
        public int Id { get; }

        public string Note { get; set; }

        public ClientNoteResult(ClientNote clientNote)
        {
            Id = clientNote.Id;
            Note = clientNote.Note;
        }
    }
}