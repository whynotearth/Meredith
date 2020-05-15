using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Results.Api.v0.Hotel.Price;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Hotel
{
    [ApiVersion("0")]
    [Route("api/v0/hotel/roomtypes")]
    [ProducesErrorResponseType(typeof(void))]
    public class RoomTypeController : ControllerBase
    {
        private readonly MeredithDbContext _meredithDbContext;

        public RoomTypeController(MeredithDbContext meredithDbContext)
        {
            _meredithDbContext = meredithDbContext;
        }

        [HttpGet("{roomTypeId}/prices/")]
        public async Task<ActionResult<List<PricesResult>>> Prices(int roomTypeId, DateTime startDate, DateTime endDate)
        {
            var prices = await _meredithDbContext.Prices
                .OfType<HotelPrice>()
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