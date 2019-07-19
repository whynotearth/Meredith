namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.Data.Entity;

    [ApiVersion("0")]
    [Route("/api/v0/roomtypes")]
    [EnableCors]
    public class RoomTypeController : Controller
    {
        private MeredithDbContext MeredithDbContext { get; }

        public RoomTypeController(MeredithDbContext meredithDbContext)
        {
            MeredithDbContext = meredithDbContext;
        }

        [HttpGet]
        [Route("{roomTypeId}/prices/")]
        public async Task<IActionResult> Prices(int roomTypeId, DateTime startDate, DateTime endDate)
        {
            var prices = await MeredithDbContext.Prices
                .Where(p => p.RoomTypeId == roomTypeId
                    && p.Date >= startDate
                    && p.Date <= endDate)
                .Select(p => new
                {
                    p.Id,
                    p.RoomTypeId,
                    p.Date,
                    p.Amount
                })
                .ToListAsync();
            return Ok(prices);
        }
    }
}