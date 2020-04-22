using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Models.Api.v0.Salon;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Salon
{
    [ApiVersion("0")]
    [Route("api/v0/tenants/{tenantSlug}")]
    [ProducesErrorResponseType(typeof(void))]
    public class TenantReservationController : ControllerBase
    {
        private readonly MeredithDbContext _meredithDbContext;
        private readonly ReservationService _reservationService;
        private readonly UserManager _userManager;

        public TenantReservationController(MeredithDbContext meredithDbContext, ReservationService reservationService,
            UserManager userManager)
        {
            _meredithDbContext = meredithDbContext;
            _reservationService = reservationService;
            _userManager = userManager;
        }

        [Authorize]
        [Returns200]
        [Returns400]
        [Returns401]
        [Returns404]
        [HttpPost("reservations")]
        public async Task<IActionResult> Reserve(string tenantSlug, TenantReservationModel model)
        {
            if (model.DeliveryDateTime < DateTime.UtcNow)
            {
                return BadRequest("Invalid delivery date");
            }

            var tenant =
                await _meredithDbContext.Tenants.FirstOrDefaultAsync(t => t.Slug.ToLower() == tenantSlug.ToLower());

            if (tenant is null)
            {
                return NotFound($"Tenant {tenantSlug} not found");
            }

            var userId = _userManager.GetUserId(User);

            _reservationService.Reserve(tenant.Id, model.Orders.Select(i => i.ToString()).ToList(), model.SubTotal,
                model.DeliveryFee, model.Amount, model.Tax, model.DeliveryDateTime, model.UserTimeZoneOffset,
                model.PaymentMethod, model.Message, userId);

            return Ok();
        }
    }
}