using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Models.Api.v0.Salon;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Salon;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Salon
{
    [ApiVersion("0")]
    [Route("api/v0/salon/reservations")]
    public class SalonReservationController : ControllerBase
    {
        private readonly MeredithDbContext _meredithDbContext;
        private readonly ReservationService _reservationService;
        private readonly UserManager _userManager;

        public SalonReservationController(MeredithDbContext meredithDbContext, ReservationService reservationService,
            UserManager userManager)
        {
            _meredithDbContext = meredithDbContext;
            _reservationService = reservationService;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        [Route("{tenantId}/{pageSlug}/reserve")]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reserve(int tenantId, string pageSlug, SalonReservationModel model)
        {
            var page = await _meredithDbContext.Pages.Include(item => item.Company).FirstOrDefaultAsync(p =>
                p.TenantId == tenantId && p.Slug.ToLower() == pageSlug.ToLower());

            if (page is null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            _reservationService.Reserve(tenantId, pageSlug, model.Orders.Select(i => i.ToString()).ToList(), model.SubTotal, model.DeliveryFee,
                model.Amount, model.DeliveryDateTime, model.Message, userId);

            return Ok();
        }
    }
}