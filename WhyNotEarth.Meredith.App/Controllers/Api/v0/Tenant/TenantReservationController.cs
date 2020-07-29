using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Tenant;
using WhyNotEarth.Meredith.Tenant.Models;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Tenant
{
    [ApiVersion("0")]
    [Route("api/v0/tenants/{tenantSlug}")]
    [ProducesErrorResponseType(typeof(void))]
    public class TenantReservationController : BaseController
    {
        private readonly ReservationService _reservationService;
        private readonly IUserService _userService;

        public TenantReservationController(ReservationService reservationService, IUserService userService)
        {
            _reservationService = reservationService;
            _userService = userService;
        }

        [Authorize]
        [Returns200]
        [Returns400]
        [Returns401]
        [Returns404]
        [HttpPost("reservations")]
        public async Task<IActionResult> Reserve(string tenantSlug, TenantReservationModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _reservationService.ReserveAsync(tenantSlug, model, user);

            return Ok();
        }
    }
}