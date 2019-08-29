namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using WhyNotEarth.Meredith.Hotel;

    [ApiVersion("0")]
    [Route("/api/v0/roomtypes")]
    [EnableCors]
    public class RoomTypeController : Controller
    {
        private RoomTypeService RoomTypeService { get; }

        public RoomTypeController(
            RoomTypeService roomTypeService)
        {
            RoomTypeService = roomTypeService;
        }

        [HttpGet]
        [Route("{roomTypeId}/prices/")]
        public async Task<IActionResult> Prices(int roomTypeId, DateTime startDate, DateTime endDate)
        {
            var prices = await RoomTypeService.GetPrices(roomTypeId, startDate, endDate);
            return Ok(prices);
        }

        [HttpGet]
        [Route("{roomTypeId}/availability/")]
        public async Task<IActionResult> Availability(int roomTypeId, DateTime startDate, DateTime endDate)
        {
            return Ok();
        }
    }
}