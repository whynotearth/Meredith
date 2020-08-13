using System;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public class ClientNote
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; } = null!;

        public string Note { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}