namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using WhyNotEarth.Meredith.App.Models.Api.v0.Price;
    using WhyNotEarth.Meredith.Hotel;

    [ApiVersion("0")]
    [Route("/api/v0/prices")]
    public class PriceController : ControllerBase
    {
        private PriceService PriceService { get; }

        public PriceController(PriceService priceService)
        {
            PriceService = priceService;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(PriceModel price)
        {
            var newPrice = await PriceService.CreatePriceAsync(price.Amount, price.Date, price.RoomTypeId);
            return Ok(new { PriceId = newPrice.Id });
        }
    }
}
