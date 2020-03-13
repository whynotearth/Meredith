using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Results.Api.v0.Price;
using WhyNotEarth.Meredith.Data.Entity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    [ApiVersion("0")]
    [Route("/api/v0/roomtypes")]
    public class RoomTypeController : ControllerBase
    {
        private MeredithDbContext MeredithDbContext { get; }

        public RoomTypeController(MeredithDbContext meredithDbContext)
        {
            MeredithDbContext = meredithDbContext;
        }

        [Route("{roomTypeId}/prices/")]
        [HttpGet]
        [ProducesResponseType(typeof(List<PricesResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Prices(int roomTypeId, DateTime startDate, DateTime endDate)
        {
            var prices = await MeredithDbContext.Prices
                .Where(p => p.RoomTypeId == roomTypeId &&
                            startDate <= p.Date && p.Date <= endDate &&
                            p.RoomType.Rooms.Count(room => !room.Reservations.Any(
                                re => startDate <= re.Start && re.End <= endDate)) > 0)
                .Select(p => new
                {
                    p.Id,
                    p.RoomTypeId,
                    p.Date,
                    p.Amount
                })
                .ToListAsync();

            var result = prices.Select(item => new PricesResult(item.Id, item.RoomTypeId, item.Date, item.Amount))
                .ToList();

            return Ok(result);
        }
    }
}