namespace WhyNotEarth.Meredith.Tests.Hotel.ReservationServiceTests
{
    using WhyNotEarth.Meredith.Tests.Data;
    using WhyNotEarth.Meredith.Hotel;
    using Xunit;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using WhyNotEarth.Meredith.Exceptions;
    using Microsoft.EntityFrameworkCore;

    public class ReservationTests : DatabaseContextTest
    {
        private HotelUtility HotelUtility { get; }

        private ReservationService ReservationService { get; }

        private DateTime Start { get; } = new DateTime(2019, 1, 1);

        public ReservationTests()
        {
            ReservationService = ServiceProvider.GetRequiredService<ReservationService>();
        }

        [Fact]
        public async Task ThrowsInvalidRoomType()
        {
            await Assert.ThrowsAsync<RecordNotFoundException>(async () => await ReservationService.CreateReservation(0, Start, Start, string.Empty, string.Empty, string.Empty, string.Empty, 0));
        }

        [Fact]
        public async Task ThrowsNotAvailable()
        {
            var roomType = await HotelUtility.CreateRoomType(Start, 10);
            await ReservationService.CreateReservation(roomType.Id, Start, Start.AddDays(1), string.Empty, string.Empty, string.Empty, string.Empty, 0);
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () => await ReservationService.CreateReservation(roomType.Id, Start, Start.AddDays(1), string.Empty, string.Empty, string.Empty, string.Empty, 0));
            Assert.Equal("There are no rooms available of this type", exception.Message);
        }

        [Fact]
        public async Task ThrowsInvalidDate()
        {
            var roomType = await HotelUtility.CreateRoomType(Start, 10);
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () => await ReservationService.CreateReservation(roomType.Id, Start.AddDays(1), Start, string.Empty, string.Empty, string.Empty, string.Empty, 0));
            Assert.Equal("Start Date cannot be before End Date", exception.Message);
        }

        [Fact]
        public async Task ThrowsNotAllPricesAvailable()
        {
            var roomType = await HotelUtility.CreateRoomType(Start, 10);
            DbContext.Prices.Remove(await DbContext.Prices.FirstOrDefaultAsync(p => p.Date == Start.AddDays(1) && p.RoomTypeId == roomType.Id));
            await DbContext.SaveChangesAsync();
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () => await ReservationService.CreateReservation(roomType.Id, Start, Start.AddDays(10), string.Empty, string.Empty, string.Empty, string.Empty, 0));
            Assert.Equal("Not all days have prices set", exception.Message);
        }
    }
}