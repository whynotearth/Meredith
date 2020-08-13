using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Tests.Data;
using Xunit;

namespace WhyNotEarth.Meredith.Tests.Hotel.PriceServiceTests
{
    public class PriceTests : DatabaseContextTest
    {
        public PriceTests()
        {
            PriceService = ServiceProvider.GetRequiredService<PriceService>();
        }

        private PriceService PriceService { get; }

        private DateTime Date { get; } = new DateTime(2020, 1, 1);

        private async Task<RoomType> CreateRoomType()
        {
            var roomType = new RoomType
            {
                Name = "Test"
            };

            DbContext.RoomTypes.Add(roomType);
            await DbContext.SaveChangesAsync();
            return roomType;
        }

        [Fact]
        public async Task PriceCannotBeBelowZero()
        {
            var roomType = await CreateRoomType();
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () =>
                await PriceService.CreatePriceAsync(-1, Date, roomType.Id));
            Assert.Equal("Amount cannot be below zero", exception.Message);
        }

        [Fact]
        public async Task ThrowsInvalidRoomType()
        {
            var exception =
                await Assert.ThrowsAsync<InvalidActionException>(async () =>
                    await PriceService.CreatePriceAsync(-1, Date, 0));
            Assert.Equal("Roomtype does not exist", exception.Message);
        }
    }
}