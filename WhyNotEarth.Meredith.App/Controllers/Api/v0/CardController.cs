namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.App.Models.Api.V0.Page;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Pages;

    [ApiVersion("0")]
    [Route("/api/v0/cards")]
    public class CardController : ControllerBase
    {
        protected MeredithDbContext MeredithDbContext { get; }

        protected StoryService StoryService { get; }

        public CardController(
            StoryService storyService,
            MeredithDbContext meredithDbContext)
        {
            StoryService = storyService;
            MeredithDbContext = meredithDbContext;
        }

        [HttpGet]
        [Route("{id}/related")]
        public async Task<IActionResult> Related(int id)
        {
            return Ok(await MeredithDbContext.Cards
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
                    Type = StoryService.GetCardType(c.CardType)
                })
                .ToListAsync());
        }
    }
}