using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Models.Api.V0.Page;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Pages;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/cards")]
    [ProducesErrorResponseType(typeof(void))]
    public class CardController : ControllerBase
    {
        private readonly MeredithDbContext _meredithDbContext;
        private readonly StoryService _storyService;

        public CardController(StoryService storyService, MeredithDbContext meredithDbContext)
        {
            _storyService = storyService;
            _meredithDbContext = meredithDbContext;
        }

        [HttpGet("{id}/related")]
        public async Task<IActionResult> Related(int id)
        {
            return Ok(await _meredithDbContext.Cards
                .Where(c => c.Id == id)
                .Select(c => new Story
                {
                    Content = c.Text,
                    CtaText = c.CallToAction,
                    CtaLink = c.CallToActionUrl,
                    Id = c.Id,
                    Image = c.BackgroundUrl,
                    PosterUrl = c.PosterUrl,
                    Blur = "2px",
                    Type = _storyService.GetCardType(c.CardType)
                })
                .ToListAsync());
        }
    }
}