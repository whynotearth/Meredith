using System.Security.Claims;
using Moq;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Services;
using WhyNotEarth.Meredith.Stripe;

namespace WhyNotEarth.Meredith.Tests.Hotel.ReservationServiceTests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using WhyNotEarth.Meredith.Tests.Data;
    using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
    using WhyNotEarth.Meredith.Exceptions;
    using WhyNotEarth.Meredith.Hotel;
    using Xunit;

    public class ReservationTests : DatabaseContextTest
    {
        private ReservationService ReservationService { get; }

        private DateTime Start { get; } = new DateTime(2020, 1, 1);

        public ReservationTests()
        {
            ReservationService = ServiceProvider.GetRequiredService<ReservationService>();
        }

        public static object[][] ValidReservationTests = new[]{
            new object [] { new DateTime(2020, 8, 30, 6, 16, 3, 304, DateTimeKind.Utc), new DateTime(2020, 9, 30, 6, 16, 3, 339, DateTimeKind.Utc) }
        };

        [Theory(Skip = "Need valid dates")]
        [MemberData(nameof(ValidReservationTests))]
        public async Task ValidReservations(DateTime start, DateTime end)
        {
            var roomType = await CreateRoomType(10);
            var reservation = await ReservationService.CreateReservation(roomType.Id, start, end, string.Empty, string.Empty, string.Empty, default, string.Empty, 0);
            Assert.NotNull(reservation);
        }

        [Fact]
        public async Task ThrowsInvalidRoomType()
        {
            await Assert.ThrowsAsync<RecordNotFoundException>(async () => await ReservationService.CreateReservation(0, Start, Start, string.Empty, string.Empty, string.Empty, default, string.Empty, 0));
        }

        [Fact]
        public async Task ThrowsNotAvailable()
        {
            var claimsPrincipal = ServiceProvider.GetRequiredService<ClaimsPrincipal>();
            var userManager = ServiceProvider.GetRequiredService<UserManager>();
            var stripeService = new Mock<IStripeService>().Object;
            var emailService = new Mock<IEmailService>().Object;
            var resevationService = new ReservationService(DbContext, claimsPrincipal, userManager, stripeService, emailService);
            var roomType = await CreateRoomType(10);
            await resevationService.CreateReservation(roomType.Id, Start, Start.AddDays(1), string.Empty, string.Empty, string.Empty, default, string.Empty, 0);
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () => await ReservationService.CreateReservation(roomType.Id, Start, Start.AddDays(1), string.Empty, string.Empty, string.Empty, default, string.Empty, 0));
            Assert.Equal("There are no rooms available of this type", exception.Message);
        }

        [Fact]
        public async Task ThrowsInvalidDate()
        {
            var roomType = await CreateRoomType(10);
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () => await ReservationService.CreateReservation(roomType.Id, Start.AddDays(1), Start, string.Empty, string.Empty, string.Empty, default, string.Empty, 0));
            Assert.Equal("Invalid number of days to reserve", exception.Message);
        }

        [Fact]
        public async Task ThrowsNotAllPricesAvailable()
        {
            var roomType = await CreateRoomType(10);
            DbContext.Prices.Remove(await DbContext.Prices.FirstOrDefaultAsync(p => p.Date == Start.AddDays(1) && p.RoomTypeId == roomType.Id));
            await DbContext.SaveChangesAsync();
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () => await ReservationService.CreateReservation(roomType.Id, Start, Start.AddDays(10), string.Empty, string.Empty, string.Empty, default, string.Empty, 0));
            Assert.Equal("Not all days have prices set", exception.Message);
        }

        private async Task<RoomType> CreateRoomType(int days = 0)
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
                    Date = Start.AddDays(i)
                });
            }

            for (var i = 0; i < 1; i++)
            {
                roomType.Rooms.Add(new Room { Number = i.ToString() });
            }

            DbContext.RoomTypes.Add(roomType);
            await DbContext.SaveChangesAsync();
            return roomType;
        }
    }
}