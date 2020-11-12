using System;
using WhyNotEarth.Meredith.Twilio;

namespace WhyNotEarth.Meredith.BrowTricks
{
    internal class FormNotifications
    {
        public ShortMessage GetConsentNotification(Public.Tenant tenant, Client client, string formUrl)
        {
            return new ShortMessage
            {
                CompanyId = tenant.CompanyId,
                TenantId = tenant.Id,
                Body =
                    $"{tenant.Name} needs the following paperwork to be completed. It should take about 5 minutes.\r\n\r\n{formUrl}",
                To = client.PhoneNumber!,
                IsWhatsApp = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public ShortMessage GetCompletionNotification(Public.Tenant tenant, Client client, string callbackUrl)
        {
            return new ShortMessage
            {
                CompanyId = tenant.CompanyId,
                TenantId = tenant.Id,
                Body =
                    $"You have completed your consent form for {tenant.Name}. Click below to view your completed consent form.\r\n\r\n{callbackUrl}",
                To = client.PhoneNumber!,
                IsWhatsApp = false,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}