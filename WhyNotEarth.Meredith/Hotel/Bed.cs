namespace WhyNotEarth.Meredith.Hotel
{
    public class Bed
    {
        public int Id { get; set; }

        public int RoomTypeId { get; set; }

        public RoomType RoomType { get; set; } = null!;

        public BedTypes BedType { get; set; }

        public int Count { get; set; }
    }

    public enum BedTypes
    {
        King,
        Queen,
        Twin,
        Single,
        LargeTwin,
        Double
    }
}