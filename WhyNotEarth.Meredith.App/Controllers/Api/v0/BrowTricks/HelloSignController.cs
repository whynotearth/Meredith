using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.BrowTricks
{
    [ApiVersion("0")]
    [Route("api/v0/hellosign")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesErrorResponseType(typeof(void))]
    public class HelloSignController : BaseController
    {
        private readonly IPmuService _pmuService;

        public HelloSignController(IPmuService pmuService)
        {
            _pmuService = pmuService;
        }

        [Returns200]
        [HttpPost("callback")]
        public ActionResult HelloSignCallback([FromForm] string json)
        {
            _pmuService.ProcessHelloSignCallback(json);

            return Ok("Hello API Event Received");
        }
    }
}