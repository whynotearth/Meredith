using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Tenant.Models;

namespace WhyNotEarth.Meredith.Tenant
{
    public class TenantReservationNotification
    {
        private const string WhatsAppOrderSmsTemplateName = "ReservationWhatsAppMessageTemplate.txt";

        public EmailMessage GetEmailMessage(Data.Entity.Models.Tenant tenant, TenantReservationModel model, User user)
        {
            var recipients = new List<Tuple<string, string?>>
            {
                Tuple.Create<string, string?>(user.Email, user.FullName),
                Tuple.Create<string, string?>(tenant.Owner.Email, tenant.Owner.FullName)
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
                orderProducts = string.Join("<br />", model.Orders.Select(item => item.ToString())),
                subTotal = model.SubTotal,
                deliveryFee = model.DeliveryFee,
                amount = model.Amount,
                tax = model.Tax,
                deliveryAddress = user.Address,
                name = user.FullName,
                phone = user.PhoneNumber,
                email = user.Email,
                paymentMethod = model.PaymentMethod,
                deliveryTime = model.DeliveryDateTime.InZone(model.UserTimeZoneOffset, "ddd, d MMM hh:mm"),
                message = model.Message ?? string.Empty,
                googleMaps = user.GoogleLocation
            };

            var emailInfo = new EmailMessage(tenant.Company.Id, recipients)
            {
                TemplateData = templateData
            };

            return emailInfo;
        }

        public List<ShortMessage> GetWhatsAppMessage(Data.Entity.Models.Tenant tenant, TenantReservationModel model,
            User user)
        {
            var recipients = new List<string>
            {
                user.PhoneNumber,
                tenant.Owner.PhoneNumber
            };

            var template = new StringBuilder(GetWhatsAppTemplate());

            template.Replace("{orders}", string.Join("\n", model.Orders.Select(item => item.ToString())));
            template.Replace("{subTotal}", model.SubTotal.ToString(CultureInfo.InvariantCulture));
            template.Replace("{tax}", model.Tax.ToString(CultureInfo.InvariantCulture));
            template.Replace("{deliveryFee}", model.DeliveryFee.ToString(CultureInfo.InvariantCulture));
            template.Replace("{amount}", model.Amount.ToString(CultureInfo.InvariantCulture));
            template.Replace("{deliveryAddress}", user.Address);
            template.Replace("{name}", user.FullName);
            template.Replace("{phone}", user.PhoneNumber);
            template.Replace("{email}", user.Email);
            template.Replace("{paymentMethod}", model.PaymentMethod);
            template.Replace("{deliveryTime}",
                model.DeliveryDateTime.InZone(model.UserTimeZoneOffset, "ddd, d MMM hh:mm"));
            template.Replace("{message}", !string.IsNullOrEmpty(model.Message) ? model.Message : "N/A");
            template.Replace("{tenantName}", tenant.Name.ToUpper());
            template.Replace("{tenantGooglemaps}", tenant.Owner.GoogleLocation);
            template.Replace("{tenantPhone}", tenant.Owner.PhoneNumber);
            template.Replace("{tenantEmail}", tenant.Owner.Email);

            return recipients.Select(item =>
                new ShortMessage
                {
                    CompanyId = tenant.CompanyId,
                    TenantId = tenant.Id,
                    To = item,
                    Body = template.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    IsWhatsApp = true
                }).ToList();
        }

        private string GetWhatsAppTemplate()
        {
            var assembly = typeof(TenantReservationNotification).GetTypeInfo().Assembly;

            var name = assembly.GetManifestResourceNames()
                .FirstOrDefault(item => item.EndsWith(WhatsAppOrderSmsTemplateName));
            var stream = assembly.GetManifestResourceStream(name);

            if (stream is null)
            {
                throw new Exception("Missing resource.");
            }

            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}