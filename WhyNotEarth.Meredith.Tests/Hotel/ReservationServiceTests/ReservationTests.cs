using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Persistence;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Services;
using Xunit;

namespace WhyNotEarth.Meredith.Tests.Hotel.ReservationServiceTests
{
    public class ReservationTests
    {
        public DateTime Start { get; set; } = new DateTime(2020, 1, 1);

        public static object[][] ValidReservationTests =
        {
            new object[]
            {
                new DateTime(2020, 8, 30, 6, 16, 3, 304, DateTimeKind.Utc),
                new DateTime(2020, 9, 30, 6, 16, 3, 339, DateTimeKind.Utc)
            }
        };

        [Theory]
        [MemberData(nameof(ValidReservationTests))]
        public async Task ValidReservations(DateTime start, DateTime end)
        {
            // Arrange
            var (dbContext, reservationService) = GetServices(nameof(ValidReservations));

            // We need a throwaway room to possibly introduce a collision
            await CreateRoomType(dbContext, start, 31);
            var roomType = await CreateRoomType(dbContext, start, 31);
            var reservation = await reservationService.CreateReservation(roomType.Id, start, end, string.Empty,
                string.Empty, string.Empty, string.Empty, string.Empty, 0);
            Assert.NotNull(reservation);
        }

        private async Task<RoomType> CreateRoomType(MeredithDbContext dbContext,  DateTime startDate, int days = 0)
        {
            var roomType = new RoomType
            {
                Name = "Test"
            };
            for (var i = 0; i < days; i++)
            {
                roomType.Prices.Add(new HotelPrice
                {
                    Amount = 10,
                    Date = startDate.AddDays(i)
                });
            }

            for (var i = 0; i < 1; i++)
            {
                roomType.Rooms.Add(new Room { Number = i.ToString() });
            }

            dbContext.RoomTypes.Add(roomType);
            await dbContext.SaveChangesAsync();
            return roomType;
        }

        private (MeredithDbContext, ReservationService) GetServices(string testName)
        {
            var options = new DbContextOptionsBuilder<MeredithDbContext>()
                .UseInMemoryDatabase(testName)
                .Options;

            var meredithDbContext = new MeredithDbContext(options);

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(Mock.Of<User>());

            return (meredithDbContext, new ReservationService(meredithDbContext, Mock.Of<ClaimsPrincipal>(),
                userServiceMock.Object, Mock.Of<IStripeService>(), Mock.Of<IEmailService>()));
        }

        [Fact]
        public async Task ThrowsInvalidDate()
        {
            // Arrange
            var (dbContext, reservationService) = GetServices(nameof(ThrowsInvalidDate));

            var roomType = await CreateRoomType(dbContext, Start, 10);
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () =>
                await reservationService.CreateReservation(roomType.Id, Start.AddDays(1), Start, string.Empty,
                    string.Empty, string.Empty, string.Empty, string.Empty, 0));
            Assert.Equal("Invalid number of days to reserve", exception.Message);
        }

        [Fact]
        public async Task ThrowsInvalidRoomType()
        {
            // Arrange
            var (_, reservationService) = GetServices(nameof(ThrowsInvalidRoomType));

            await Assert.ThrowsAsync<RecordNotFoundException>(async () =>
                await reservationService.CreateReservation(0, Start, Start, string.Empty, string.Empty, string.Empty,
                    string.Empty, string.Empty, 0));
        }

        [Fact]
        public async Task ThrowsNotAllPricesAvailable()
        {
            // Arrange
            var (dbContext, reservationService) = GetServices(nameof(ThrowsNotAllPricesAvailable));

            var roomType = await CreateRoomType(dbContext, Start, 10);
            dbContext.Prices.Remove(await dbContext.Prices.OfType<HotelPrice>().FirstOrDefaultAsync(p =>
                p.Date == Start.AddDays(1) && p.RoomTypeId == roomType.Id));
            await dbContext.SaveChangesAsync();
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () =>
                await reservationService.CreateReservation(roomType.Id, Start, Start.AddDays(10), string.Empty,
                    string.Empty, string.Empty, string.Empty, string.Empty, 0));
            Assert.Equal("Not all days have prices set", exception.Message);
        }

        [Fact]
        public async Task ThrowsNotAvailable()
        {
            // Arrange
            var (dbContext, reservationService) = GetServices(nameof(ThrowsNotAvailable));

            var roomType = await CreateRoomType(dbContext, Start, 10);
            await reservationService.CreateReservation(roomType.Id, Start, Start.AddDays(1), string.Empty,
                string.Empty,
                string.Empty, string.Empty, string.Empty, 0);
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () =>
                await reservationService.CreateReservation(roomType.Id, Start, Start.AddDays(1), string.Empty,
                    string.Empty, string.Empty, string.Empty, string.Empty, 0));
            Assert.Equal("There are no rooms available of this type", exception.Message);
        }
    }
}