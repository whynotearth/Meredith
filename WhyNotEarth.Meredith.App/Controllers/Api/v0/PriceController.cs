namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using WhyNotEarth.Meredith.App.Models.Api.v0.Price;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Exceptions;
    using WhyNotEarth.Meredith.Hotel;

    [ApiVersion("0")]
    [Route("/api/v0/prices")]
    [EnableCors]
    public class PriceController : Controller
    {
        private MeredithDbContext MeredithDbContext { get; }
        private PriceService PriceService { get; }

        public PriceController(MeredithDbContext meredithDbContext, PriceService priceService)
        {
            PriceService = priceService;
            MeredithDbContext = meredithDbContext;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(PriceModel price)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var newPrice = await PriceService.CreatePriceAsync(price.Amount, price.Date, price.RoomTypeId);
                return Ok(new { PriceId = newPrice.Id });
            }
            catch (InvalidActionException e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }
    }
}
