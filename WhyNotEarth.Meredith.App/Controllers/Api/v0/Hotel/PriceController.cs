using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Models.Api.v0.Price;
using WhyNotEarth.Meredith.Hotel;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Hotel
{
    [ApiVersion("0")]
    [Route("api/v0/hotel/prices")]
    [ProducesErrorResponseType(typeof(void))]
    public class PriceController : ControllerBase
    {
        private readonly PriceService _priceService;

        public PriceController(PriceService priceService)
        {
            _priceService = priceService;
        }

        [HttpPost("")]
        public async Task<IActionResult> Create(PriceModel price)
        {
            var newPrice = await _priceService.CreatePriceAsync(price.Amount, price.Date, price.RoomTypeId);

            return Ok(new {PriceId = newPrice.Id});
        }
    }
}