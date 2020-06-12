using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Sms;

namespace WhyNotEarth.Meredith.Tenant
{
    public class ReservationService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _meredithDbContext;
        private readonly SendGridService _sendGridService;
        private readonly UserManager _userManager;
        private readonly TwilioService _twilioService;

        public ReservationService(MeredithDbContext meredithDbContext, SendGridService sendGridService,
            IBackgroundJobClient backgroundJobClient, UserManager userManager, TwilioService twilioService)
        {
            _meredithDbContext = meredithDbContext;
            _sendGridService = sendGridService;
            _backgroundJobClient = backgroundJobClient;
            _userManager = userManager;
            _twilioService = twilioService;
        }

        public void Reserve(int tenantId, List<string> orders, decimal subTotal, decimal deliveryFee, decimal amount,
            decimal tax, DateTime deliveryDateTime, int userTimeZoneOffset, string paymentMethod, string? message,
            string userId, bool? whatsappNotification)
        {
            if (whatsappNotification == true)
            {
                _backgroundJobClient.Enqueue<ReservationService>(service =>
               service.SendWhatsappSms(tenantId, orders, subTotal, deliveryFee, amount, tax, deliveryDateTime,
                   userTimeZoneOffset, paymentMethod, message, userId));
            }
            else
            {
                _backgroundJobClient.Enqueue<ReservationService>(service =>
                    service.SendEmail(tenantId, orders, subTotal, deliveryFee, amount, tax, deliveryDateTime,
                        userTimeZoneOffset, paymentMethod, message, userId));
            }
        }

        public async Task SendEmail(int tenantId, List<string> orders, decimal subTotal, decimal deliveryFee,
            decimal amount, decimal tax, DateTime deliveryDateTime, int userTimeZoneOffset, string paymentMethod,
            string? message, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                throw new Exception($"Cannot find the user: {userId}");
            }

            var tenant = await _meredithDbContext.Tenants
                .Include(item => item.Owner)
                .Include(item => item.Company)
                .FirstOrDefaultAsync(item => item.Id == tenantId);

            if (tenant is null)
            {
                throw new Exception($"Cannot find the tenant: {tenantId}");
            }

            if (tenant.Company is null)
            {
                throw new Exception($"Tenant: {tenantId} is not connected to any company");
            }

            if (tenant.Owner is null)
            {
                throw new Exception($"Tenant: {tenantId} does not have an owner");
            }

            var recipients = new List<Tuple<string, string?>>
            {
                Tuple.Create<string, string?>(user.Email, user.Name),
                Tuple.Create<string, string?>(tenant.Owner.Email, tenant.Owner.Name)
            };

            var templateData = new
            {
                subject = $"your order with {tenant.Company.Name}",
                tenant = new
                {
                    h2 = tenant.Name,
                    phone = tenant.Owner.PhoneNumber,
                    email = tenant.Owner.Email
                },
                orderProducts = string.Join("<br />", orders),
                subTotal =  subTotal,
                deliveryFee =  deliveryFee,
                amount = amount,
                tax = tax,
                deliveryAddress = user.Address,
                name = user.Name,
                phone = user.PhoneNumber,
                email = user.Email,
                paymentMethod = paymentMethod,
                deliveryTime = deliveryDateTime.InZone(userTimeZoneOffset, "ddd, d MMM hh:mm"),
                message = message ?? string.Empty,
                googleMaps = user.GoogleLocation
            };

            var emailInfo = new EmailInfo(tenant.Company.Id, recipients)
            {
                TemplateData = templateData
            };

            await _sendGridService.SendEmailAsync(emailInfo);
        }
        public async Task SendWhatsappSms(int tenantId, List<string> orders, decimal subTotal, decimal deliveryFee,
            decimal amount, decimal tax, DateTime deliveryDateTime, int userTimeZoneOffset, string paymentMethod,
            string? message, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var tenant = await _meredithDbContext.Tenants
                .Include(item => item.Owner)
                .FirstOrDefaultAsync(item => item.Id == tenantId);

            var recepients = new List<string>()
            {
                user.PhoneNumber,
                tenant.Owner.PhoneNumber
            };

            StringBuilder formatReader = new StringBuilder(_twilioService.GetWhatsappSmsTemplate());

            formatReader.Replace("{orders}", string.Join("\n", orders));
            formatReader.Replace("{subTotal}", subTotal.ToString());
            formatReader.Replace("{tax}", tax.ToString());
            formatReader.Replace("{deliveryFee}", deliveryFee.ToString());
            formatReader.Replace("{amount}", amount.ToString());
            formatReader.Replace("{deliveryAddress}", user.Address);
            formatReader.Replace("{name}", user.Name);
            formatReader.Replace("{phone}", user.PhoneNumber);
            formatReader.Replace("{email}", user.Email);
            formatReader.Replace("{paymentMethod}", paymentMethod);
            formatReader.Replace("{deliveryTime}", deliveryDateTime.InZone(userTimeZoneOffset, "ddd, d MMM hh:mm"));
            formatReader.Replace("{message}", !string.IsNullOrEmpty(message) ? message : "N/A");
            formatReader.Replace("{tenantName}", tenant.Name.ToUpper());
            formatReader.Replace("{tenantGooglemaps}", tenant.Owner.GoogleLocation);
            formatReader.Replace("{tenantPhone}", tenant.Owner.PhoneNumber);
            formatReader.Replace("{tenantEmail}", tenant.Owner.Email);

            await _twilioService.SendWhatsapp(formatReader.ToString(), recepients);
        }
    }
}