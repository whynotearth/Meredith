using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Controllers.Api.v0;
using WhyNotEarth.Meredith.App.Results.Api.v0.Price;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
using Xunit;

namespace WhyNotEarth.Meredith.Tests.Controllers
{
    public class RoomTypeControllerTests
    {
        [Fact]
        public async Task Prices_ReturnsEmptyResult_WithInvalidDates()
        {
            // Arrange
            await using var meredithDbContext = await InitializeDb(nameof(Prices_ReturnsEmptyResult_WithInvalidDates));

            var controller = new RoomTypeController(meredithDbContext);

            // Act
            var result = await controller.Prices(1, new DateTime(2020, 2, 2), new DateTime(2020, 2, 4));

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<PricesResult>>(viewResult.Value);
            Assert.Empty(model);
        }

        [Fact]
        public async Task Prices_ReturnsEmptyResult_WithInvalidRoomTypeId()
        {
            // Arrange
            await using var meredithDbContext = await InitializeDb(nameof(Prices_ReturnsEmptyResult_WithInvalidRoomTypeId));

            var controller = new RoomTypeController(meredithDbContext);

            // Act
            var result = await controller.Prices(-1, new DateTime(2020, 1, 2), new DateTime(2020, 1, 4));

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<PricesResult>>(viewResult.Value);
            Assert.Empty(model);
        }

        [Fact]
        public async Task Prices_ReturnsResult_WithSpecifiedDates()
        {
            // Arrange
            await using var meredithDbContext = await InitializeDb(nameof(Prices_ReturnsResult_WithSpecifiedDates));

            var controller = new RoomTypeController(meredithDbContext);
            var expected = new List<PricesResult>
            {
                new PricesResult(2, 1, new DateTime(2020, 1, 2), 200),
                new PricesResult(3, 1, new DateTime(2020, 1, 3), 300),
                new PricesResult(4, 1, new DateTime(2020, 1, 4), 400)
            };

            // Act
            var result = await controller.Prices(1, new DateTime(2020, 1, 2), new DateTime(2020, 1, 4));

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<List<PricesResult>>(viewResult.Value);
            AssertEqual(expected, model);
        }

        private async Task<MeredithDbContext> InitializeDb(string name)
        {
            var options = new DbContextOptionsBuilder<MeredithDbContext>()
                .UseInMemoryDatabase(name)
                .Options;

            var meredithDbContext = new MeredithDbContext(options);

            // Add two roomTypes
            var roomTypes = new List<RoomType>
            {
                new RoomType
                {
                    Id = 1
                },
                new RoomType

                {
                    Id = 2
                }
            };
            meredithDbContext.RoomTypes.AddRange(roomTypes);

            // Add three rooms for each roomType
            var id = 1;
            foreach (var roomType in roomTypes)
            {
                for (var i = 0; i < 3; i++)
                {
                    meredithDbContext.Rooms.Add(new Room
                    {
                        Id = id,
                        RoomTypeId = roomType.Id
                    });

                    id++;
                }
            }

            // For each roomType add prices for 5 days from 1/1/2020
            id = 1;
            foreach (var roomType in roomTypes)
            {
                for (var i = 0; i < 5; i++)
                {
                    meredithDbContext.Prices.Add(new Price
                    {
                        Id = id,
                        Date = new DateTime(2020, 1, 1).AddDays(i),
                        RoomTypeId = roomType.Id,
                        Amount = id * 100
                    });

                    id++;
                }
            }

            meredithDbContext.Reservations.AddRange(new Reservation
                {
                    Id = 1,
                    RoomId = 1,
                    Start = new DateTime(2020, 1, 1),
                    End = new DateTime(2020, 1, 2)
                },
                new Reservation
                {
                    Id = 2,
                    RoomId = 2,
                    Start = new DateTime(2020, 1, 2),
                    End = new DateTime(2020, 1, 4)
                });

            await meredithDbContext.SaveChangesAsync();

            return meredithDbContext;
        }

        private void AssertEqual(List<PricesResult> expected, List<PricesResult> actual)
        {
            Assert.Equal(expected.Count, actual.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                AssertPricesResult(expected[i], actual[i]);
            }
        }

        private void AssertPricesResult(PricesResult expected, PricesResult actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.RoomTypeId, actual.RoomTypeId);
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Amount, actual.Amount);
        }
    }
}