using System.Collections.Generic;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Hotel.Page
{
    public class RoomTypeResult
    {
        public int Id { get; }

        public string? Name { get; }

        public int Capacity { get; }

        public List<BedResult> Beds { get; } = new List<BedResult>();

        public RoomTypeResult(int id, string? name, int capacity)
        {
            Id = id;
            Name = name;
            Capacity = capacity;
        }
    }
}