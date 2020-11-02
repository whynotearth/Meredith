using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Page;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/cards")]
    [ProducesErrorResponseType(typeof(void))]
    public class CardController : ControllerBase
    {
        private readonly IDbContext _dbContext;

        public CardController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Returns404]
        [HttpGet("{id}/related")]
        public async Task<StoryResult> Related(int id)
        {
            var card = await _dbContext.Cards.FirstOrDefaultAsync(c => c.Id == id);

            if (card is null)
            {
                throw new RecordNotFoundException();
            }

            var result = new StoryResult(card.Id, card.Text, card.CallToAction, card.CallToActionUrl,
                card.BackgroundUrl, card.PosterUrl, card.CardType);

            return result;
        }
    }
}