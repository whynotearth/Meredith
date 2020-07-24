using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.HelloSign;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.BrowTricks
{
    [ApiVersion("0")]
    [Route("api/v0/hellosign")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesErrorResponseType(typeof(void))]
    public class HelloSignController : BaseController
    {
        private readonly IHelloSignService _helloSignService;

        public HelloSignController(IHelloSignService helloSignService)
        {
            _helloSignService = helloSignService;
        }

        [Returns200]
        [HttpPost("callback")]
        public async Task<ActionResult> HelloSignCallback([FromForm] string json)
        {
            await _helloSignService.ProcessEventsAsync(json);

            return Ok();
        }
    }
}