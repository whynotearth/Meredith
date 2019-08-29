namespace WhyNotEarth.Meredith.Tests.Hotel.ReservationServiceTests
{
    using WhyNotEarth.Meredith.Tests.Data;
    using WhyNotEarth.Meredith.Hotel;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;
    using Xunit;
    using WhyNotEarth.Meredith.Exceptions;
    using System;
    using System.Linq;

    public class AvailabilityTests : HotelTest
    {
        private RoomTypeService RoomTypeService { get; }

        private DateTime Start { get; } = new DateTime(2019, 1, 1);

        public AvailabilityTests()
        {
            RoomTypeService = ServiceProvider.GetRequiredService<RoomTypeService>();
        }

        [Fact]
        public async Task ThrowIfInvalidDate()
        {
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () => await RoomTypeService.GetAvailabilitiesAsync(1, Start.AddDays(1), Start));
            Assert.Equal("Start Date cannot be before End Date", exception.Message);
        }

        [Fact]
        public async Task RoomAvailableEmpty()
        {
            var availability = await RoomTypeService.GetAvailabilitiesAsync(1, Start, Start);
            Assert.NotEmpty(availability);
            Assert.True(availability.Select(a => a.Availabile).First());
        }

        [Fact]
        public async Task RoomAvailablePartiallyFull()
        {
            var availability = await RoomTypeService.GetAvailabilitiesAsync(1, Start, Start);
            Assert.NotEmpty(availability);
            Assert.True(availability.Select(a => a.Availabile).First());
        }

        [Fact]
        public async Task RoomNotAvailableFull()
        {
            var availability = await RoomTypeService.GetAvailabilitiesAsync(1, Start, Start);
            Assert.NotEmpty(availability);
            Assert.False(availability.Select(a => a.Availabile).First());
        }
    }
}