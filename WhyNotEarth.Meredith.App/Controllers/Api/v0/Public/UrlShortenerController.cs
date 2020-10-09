using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.UrlShortener;
using WhyNotEarth.Meredith.UrlShortener;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/shortener")]
    [ProducesErrorResponseType(typeof(void))]
    public class UrlShortenerController : ControllerBase
    {
        private readonly IUrlShortenerService _urlShortenerService;

        public UrlShortenerController(IUrlShortenerService urlShortenerService)
        {
            _urlShortenerService = urlShortenerService;
        }

        [Returns200]
        [Returns404]
        [HttpGet("{id}")]
        public async Task<ActionResult<ShortUrlResult>> Get(string id)
        {
            var shortUrl = await _urlShortenerService.GetAsync(id);

            return new ShortUrlResult(shortUrl);
        }
    }
}