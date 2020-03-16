using System.Collections.Generic;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Hotel.Page
{
    public class HotelModuleResult
    {
        public int Id { get; }

        public List<string> Amenities { get; } = new List<string>();

        public string GettingAround { get; }

        public string Location { get; }

        public List<string> Rules { get; } = new List<string>();

        public List<string> Spaces { get; } = new List<string>();

        public List<RoomTypeResult> RoomTypes { get; set; } = new List<RoomTypeResult>();

        public HotelModuleResult(int id, string gettingAround, string location)
        {
            Id = id;
            GettingAround = gettingAround;
            Location = location;
        }
    }
}