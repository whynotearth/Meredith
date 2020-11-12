using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Services;

namespace WhyNotEarth.Meredith.Identity.Notifications
{
    internal class ForgotPasswordNotification : Notification
    {
        private readonly string _callbackUrl;
        private readonly Company _company;
        private readonly IResourceService _resourceService;
        private readonly User _user;

        public ForgotPasswordNotification(Company company, string callbackUrl, User user,
            IResourceService resourceService) : base(company)
        {
            _company = company;
            _callbackUrl = callbackUrl;
            _user = user;
            _resourceService = resourceService;
        }

        public override string GetMessage(NotificationType notificationType)
        {
            if (notificationType == NotificationType.Email)
            {
                return GetHtmlTemplate(_callbackUrl);
            }

            return
                $"Sorry to hear you're having trouble logging into {_company.Name}. We can help you get straight back into your account. {_callbackUrl}";
        }

        public override string GetSubject()
        {
            return $"{_user.GetDisplayName()}, we've made it easy to get back on {_company.Name}";
        }

        private string GetHtmlTemplate(string url)
        {
            var templateName = _company.Slug == BrowTricksCompany.Slug
                ? "browtricks-reset-password.html"
                : "general-reset-password.html";

            var template = _resourceService.Get(templateName, new Dictionary<string, string>
            {
                {"{{company}}", _company.Name.First().ToString().ToUpper() + _company.Name[1..]},
                {"{{url}}", HtmlEncoder.Default.Encode(url)}
            });

            return template;
        }
    }
}