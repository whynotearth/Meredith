namespace WhyNotEarth.Meredith.Hotel
{
    using System;
    using System.Threading.Tasks;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;

    public class HotelUtility
    {
        protected MeredithDbContext MeredithDbContext { get; }

        public HotelUtility(MeredithDbContext meredithDbContext)
        {
            MeredithDbContext = meredithDbContext;
        }

        public async Task<RoomType> CreateRoomType(DateTime start, int days = 0)
        {
            var roomType = new RoomType
            {
                Name = "Test"
            };
            for (var i = 0; i < days; i++)
            {
                roomType.Prices.Add(new Price
                {
                    Amount = 10,
                    Date = start.AddDays(i)
                });
            }

            for (var i = 0; i < 1; i++)
            {
                roomType.Rooms.Add(new Room { Number = i.ToString() });
            }

            MeredithDbContext.RoomTypes.Add(roomType);
            await MeredithDbContext.SaveChangesAsync();
            return roomType;
        }
    }
}