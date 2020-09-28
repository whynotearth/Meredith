using System;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Twilio;

namespace WhyNotEarth.Meredith.BrowTricks
{
    internal class FormNotifications
    {
        public ShortMessage GetConsentNotification(Public.Tenant tenant, User user, string formUrl)
        {
            return new ShortMessage
            {
                CompanyId = tenant.CompanyId,
                TenantId = tenant.Id,
                Body =
                    $"{tenant.Name} needs the following paperwork to be completed. It should take about 5 minutes.\r\n\r\n{formUrl}",
                To = user.PhoneNumber,
                IsWhatsApp = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public ShortMessage GetCompletionNotification(Public.Tenant tenant, User user, string callbackUrl)
        {
            return new ShortMessage
            {
                CompanyId = tenant.CompanyId,
                TenantId = tenant.Id,
                Body =
                    $"You have completed your consent form for {tenant.Name}. Click below to view your completed consent form.\r\n\r\n{callbackUrl}",
                To = user.PhoneNumber,
                IsWhatsApp = false,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}