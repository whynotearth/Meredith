namespace WhyNotEarth.Meredith.App.Results.Api.v0.Hotel.Page
{
    public class BedResult
    {
        public int Id { get; }

        public string Type { get; }

        public int Count { get; }

        public BedResult(int id, string type, int count)
        {
            Id = id;
            Type = type;
            Count = count;
        }
    }
}