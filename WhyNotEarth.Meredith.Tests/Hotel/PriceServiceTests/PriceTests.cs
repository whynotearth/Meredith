using WhyNotEarth.Meredith.Tests.Data;
using WhyNotEarth.Meredith.Hotel;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WhyNotEarth.Meredith.Tests.Hotel.PriceServiceTests
{
    public class PriceTests : DatabaseContextTest 
    {
        private PriceService PriceService { get; }

        private DateTime Date { get; } = new DateTime(2019, 1, 1);

        public PriceTests()
        {
            PriceService = ServiceProvider.GetRequiredService<PriceService>();
        }

        [Fact]
        public async Task PriceCannotBeBelowZero()
        {
            var roomType = await CreateRoomType();
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () => await PriceService.CreatePriceAsync(-1, Date, roomType.Id));
            Assert.Equal("Amount cannot be below zero", exception.Message);
        }

        [Fact]
        public async Task ThrowsInvalidRoomType()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await PriceService.CreatePriceAsync(-1, Date, 0));
            Assert.Equal("Roomtype 0 does not exist", exception.Message);
        }

        private async Task<RoomType> CreateRoomType(int days = 0)
        {
            var roomType = new RoomType
            {
                Name = "Test"
            };


            DbContext.RoomTypes.Add(roomType);
            await DbContext.SaveChangesAsync();
            return roomType;
        }
    }
}
