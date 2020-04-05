using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.Salon
{
    public class ReservationService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _meredithDbContext;
        private readonly SendGridService _sendGridService;
        private readonly UserManager _userManager;

        public ReservationService(MeredithDbContext meredithDbContext, SendGridService sendGridService,
            IBackgroundJobClient backgroundJobClient, UserManager userManager)
        {
            _meredithDbContext = meredithDbContext;
            _sendGridService = sendGridService;
            _backgroundJobClient = backgroundJobClient;
            _userManager = userManager;
        }

        public void Reserve(int tenantId, string pageSlug, List<string> orders, decimal subTotal,
            decimal deliveryFee, decimal amount, DateTime deliveryDateTime, string message, string userId)
        {
            _backgroundJobClient.Enqueue<ReservationService>(service =>
                service.SendEmail(tenantId, pageSlug, orders, subTotal, deliveryFee, amount, deliveryDateTime, message,
                    userId));
        }

        public async Task SendEmail(int tenantId, string pageSlug, List<string> orders, decimal subTotal,
            decimal deliveryFee, decimal amount, DateTime deliveryDateTime, string message, string userId)
        {
            var page = await _meredithDbContext.Pages.Include(item => item.Company).FirstOrDefaultAsync(p =>
                p.TenantId == tenantId && p.Slug.ToLower() == pageSlug.ToLower());

            var tenant = await _meredithDbContext.Tenants
                .Include(item => item.User)
                .Include(item => item.Company)
                .FirstOrDefaultAsync(item => item.Id == tenantId);
            var user = await _userManager.FindByIdAsync(userId);

            var templateData = new Dictionary<string, object>
            {
                {
                    "tenant", new
                    {
                        featuredImage = page.FeaturedImage,
                        h2 = page.Header,
                        phone = tenant.User.PhoneNumber,
                        email = tenant.User.Email
                    }
                },
                {"orderProducts", string.Join("<br />", orders)},
                {"subTotal", subTotal},
                {"deliveryFee", deliveryFee},
                {"amount", amount},
                {"deliveryAddress", user.Address},
                {"name", user.Name},
                {"phone", user.PhoneNumber},
                {"email", user.Email},
                {"paymentMethod", "Cash"},
                {"deliveryTime", deliveryDateTime},
                {"message", message},
                {"googleMaps", user.GoogleLocation}
            };

            await _sendGridService.SendEmail(tenant.Company.Id, user.Email, user.Name, templateData);
        }
    }
}